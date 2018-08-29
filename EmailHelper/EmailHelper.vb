Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Xml
Imports EkiXmlDocument

Public Class EmailHelper
    Public Function BuildRoomDetails(ByVal Conn As OleDbConnection,
                                     ByVal XmlReader As EXmlReader,
                                     ByVal WebSite As String,
                                     ByVal OrderNumber As String,
                                     ByVal StaffName As String,
                                     ByVal CustomerName As String) As String
        Dim RoomTypes() As String = Nothing, rcount As Integer = 0, RT() As String, Rs(2, 0) As Integer, RD(1, 0) As Date, rtindex As Integer = 0
        Dim d() As String = XmlReader.ReadValue("Email/Day").Split(","), m() As String = XmlReader.ReadValue("Email/" & XmlReader.ReadValue("Email/MonthType")).Split(",")
        Dim tmpstr As String = String.Empty, cs() As String = XmlReader.ReadValue("Email/Count").Split(","), IsSingleOrder As Boolean = True
        For Each n As XmlNode In XmlReader.ReadNodes("RoomType")
            If n.NodeType = XmlNodeType.Element Then
                ReDim Preserve RT(rcount) : RT(rcount) = n.InnerText : rcount += 1
            End If
        Next
        rcount = 0
        If XmlReader.ReadValue("Main/Local-WebSite").IndexOf(WebSite) > -1 Then
            Dim SQL As String = "SELECT DISTINCT ArrivalDate,DepartureDate,RoomType,Persons,Price,Nights FROM " & WebSite & " WHERE Status<>'Cancelled' AND OrderNumber='" & OrderNumber & "'"
            Dim CoDA As OleDbDataAdapter = New OleDbDataAdapter(SQL, Conn), ds As Data.DataSet = New Data.DataSet, roomt As Data.DataTable
            CoDA.Fill(ds, "Details") : roomt = ds.Tables("Details")
            For Each dsrow As Data.DataRow In roomt.Rows
                For i As Integer = 0 To RT.Length - 1
                    If RT(i).IndexOf(dsrow.Item(2)) > -1 Then
                        rtindex = i
                        Exit For
                    End If
                Next
                rcount = Int((dsrow.Item(3) + RT(rtindex).Substring(0, 1) - 1) / RT(rtindex).Substring(0, 1))
                tmpstr &= {" and ", ""}(Int(IsSingleOrder)) & "from " & d(dsrow.Item(0).day - 1)
                tmpstr &= {" " & m(dsrow.Item(0).Month - 1), ""}(Int(dsrow.Item(0).Month = dsrow.Item(1).Month))
                tmpstr &= " - " & d(dsrow.Item(1).Day - 1) & " " & m(dsrow.Item(1).Month - 1) & "(check out) for "
                tmpstr &= cs(rcount - 1) & " "
                tmpstr &= Regex.Match(RT(rtindex), {"(?<=,)[\s\S]+?(?=\|)", "(?<=;)[\s\S]+?(?=,)"}(Int(rcount <= 1))).Groups(0).Value
                tmpstr &= " at " & Int(Int(dsrow.Item(4).ToString.Split(",")(0)) / rcount) & " CNY/Night" & {"", "/Person"}(Int(rtindex < 3))
                IsSingleOrder = False
            Next
        Else
            Dim SQL As String = "SELECT DISTINCT RoomType,Persons,Price FROM " & WebSite & " WHERE OrderNumber='" & OrderNumber & "'"
            Dim CoDA As OleDbDataAdapter = New OleDbDataAdapter(SQL, Conn), ds As Data.DataSet = New Data.DataSet, roomt As Data.DataTable
            CoDA.Fill(ds, "Details") : roomt = ds.Tables("Details")
            For Each dsrow As Data.DataRow In roomt.Rows
                SQL = "SELECT Min(ReservedDate) As ArrivalDate,Max(ReservedDate) As DepartureDate FROM " & WebSite _
                    & " WHERE OrderNumber='" & OrderNumber & "' And Persons=" & Int(dsrow.Item(1).ToString) _
                    & " And Price=" & Int(dsrow.Item(2).ToString) & " And RoomType='" & dsrow.Item(0).ToString & "'"
                CoDA = New OleDbDataAdapter(SQL, Conn) : ds = New Data.DataSet : CoDA.Fill(ds, "Details")
                Dim datet As Data.DataTable = ds.Tables("Details")
                For i As Integer = 0 To RT.Length - 1
                    If RT(i).IndexOf(dsrow.Item(0).ToString) > -1 Then rtindex = i : Exit For
                Next
                If rtindex > 2 Then rcount = 1 Else rcount = dsrow.Item(1)
                tmpstr &= {" and ", ""}(Int(IsSingleOrder)) & "from " & d(datet.Rows.Item(0).Item(0).day - 1)
                tmpstr &= {" " & m(datet.Rows.Item(0).Item(0).Month - 1), ""}(Int(datet.Rows.Item(0).Item(0).Month = datet.Rows.Item(0).Item(1).AddDays(1).Month))
                tmpstr &= " - " & d(datet.Rows.Item(0).Item(1).AddDays(1).Day - 1) & " " & m(datet.Rows.Item(0).Item(1).AddDays(1).Month - 1) & "(check out) for "
                tmpstr &= cs({dsrow.Item(1) - 1, 0}(Int(rtindex > 2))) & " "
                tmpstr &= Regex.Match(RT(rtindex), {"(?<=,)[\s\S]+?(?=\|)", "(?<=;)[\s\S]+?(?=,)"}(Int(rcount = 1))).Groups(0).Value
                tmpstr &= " at " & dsrow.Item(2) * {1, dsrow.Item(1)}(Int(rtindex > 2)) & " CNY/Night" & {"", "/Person"}(Int(rtindex < 3))
                IsSingleOrder = False
            Next
        End If
        Return XmlReader.ReadValue("Email/EmailBody").Replace("RoomDetails", tmpstr).Replace("StaffName", StaffName).Replace("CustomerName", CustomerName)
    End Function
End Class
