Imports System.Data.OleDb
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Windows.Controls
Imports System.Xml
Imports ELite
Imports ELite.Reservation
Imports ELite.Room
Imports ELite.ELiteItem

Public Class ResHelper

    Public Function BuildRoomDetails(ByVal Conn As ELiteConnection,
                                     ByVal XmlReader As ResXmlReader,
                                     ByVal Res As ELiteListBoxResItem,
                                     ByVal StaffName As String) As String
        Dim RoomTypes() As String = Nothing, rcount As Integer = 0, Rs(2, 0) As Integer, RD(1, 0) As Date, rtindex As Integer = 0
        Dim d() As String = XmlReader.ReadValue("Email/Day").Split(","), m() As String = XmlReader.ReadValue("Email/" & XmlReader.ReadValue("Email/MonthType")).Split(",")
        Dim tmpstr As String = String.Empty, cs() As String = XmlReader.ReadValue("Email/Count").Split(","), IsSingleOrder As Boolean = True
        rcount = 0
        Dim SQL As String = "SELECT DISTINCT Type,Persons,Price,Rooms FROM info_res_rooms WHERE ResID='" & Res.ID & "'"
        Dim roomt As DataTable = Conn.Select(SQL)
        For Each dsrow As DataRow In roomt.Rows
            SQL = "SELECT Min(ReservedDate) As ArrivalDate,Max(ReservedDate) As DepartureDate FROM info_res_rooms" _
                    & " WHERE ResID='" & Res.ID & "' And Type='" & dsrow.Item(0).ToString & "'"
            Dim dateDT As DataTable = Conn.Select(SQL)
            rtindex = Int(Conn.ReadValue("select RTID from info_room_type_matches where MatchChar='" & dsrow.Item(0).ToString() + "'"))
            If rtindex > 3 Then rcount = 1 Else rcount = dsrow.Item(1)
            tmpstr &= {" and ", ""}(Int(IsSingleOrder))
            Dim arrivalDate As Date = DateTime.Parse(dateDT.Rows.Item(0).Item(0))
            Dim departureDate As Date = DateTime.Parse(dateDT.Rows.Item(0).Item(1))
            tmpstr &= "from " & d(arrivalDate.Day - 1)
            tmpstr &= {" " & m(arrivalDate.Month - 1), ""}(Int(arrivalDate.Month = departureDate.AddDays(1).Month)) & " " & arrivalDate.Year
            tmpstr &= " - " & d(departureDate.AddDays(1).Day - 1) & " " & m(departureDate.AddDays(1).Month - 1) & " " & departureDate.Year & "(check out) for "
            tmpstr &= cs({dsrow.Item(1) - 1, 0}(Int(rtindex > 2))) & " "
            tmpstr &= Conn.ReadValue("select " & {"PluralEmailCaption_en", "SingularEmailCaption_en"}(Int(rcount = 1)) &
                                     " from info_room_types where RTID=" & rtindex).ToString()
            Dim price As String = dsrow.Item(2)
            If (Res.Channel = "HostelWorld") Then price *= {1, dsrow.Item(1)}(Int(rtindex > 2))
            tmpstr &= " at " & price & " CNY/Night" & {"", "/Person"}(Int(rtindex < 3))
            IsSingleOrder = False
        Next
        'End If
        Return XmlReader.ReadValue("Email/EmailBody").Replace("RoomDetails", tmpstr).Replace("StaffName", StaffName).Replace("CustomerName", Res.FullName)
    End Function

    Public Function GetHttpHtml(ByVal HttpWebSite As String,
                                ByVal WebOrderNumber As String,
                                XmlReader As EXmlReader) As String
        Dim httpReq As HttpWebRequest, Cookies As CookieContainer
        Cookies = New CookieContainer
        httpReq = DirectCast(WebRequest.Create(New Uri(XmlReader.ReadValue(HttpWebSite & "/LoginSite"))), HttpWebRequest)
        httpReq.CookieContainer = New CookieContainer
        httpReq.Proxy = Nothing
        httpReq.Referer = XmlReader.ReadValue(HttpWebSite & "/RefererSite")
        httpReq.Method = "POST"
        httpReq.Accept = "text/html, application/xhtml+xml, */*"
        httpReq.ContentType = "application/x-www-form-urlencoded"
        httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko"
        httpReq.KeepAlive = True
        httpReq.AllowAutoRedirect = False
        httpReq.Credentials = CredentialCache.DefaultCredentials
        httpReq.Method = "POST"
        Dim PostByte() As Byte = System.Text.Encoding.GetEncoding("GB2312").GetBytes(XmlReader.ReadValue(HttpWebSite & "/PostString"))
        httpReq.ContentLength = PostByte.Length
        Dim PostStream As Stream = httpReq.GetRequestStream
        PostStream.Write(PostByte, 0, PostByte.Length)
        PostStream.Close()
        Cookies = httpReq.CookieContainer
        Dim httpRes As HttpWebResponse = httpReq.GetResponse
        For Each ck As Cookie In httpRes.Cookies
            Cookies.Add(ck)
        Next
        httpReq = DirectCast(WebRequest.Create(New Uri(XmlReader.ReadValue(HttpWebSite & "/RevSite").Replace("OrderNumber", WebOrderNumber))), HttpWebRequest)
        httpReq.CookieContainer = Cookies
        httpReq.Method = "GET"
        Dim httpResp As HttpWebResponse = CType(httpReq.GetResponse(), HttpWebResponse)
        Dim reader As StreamReader = New StreamReader(httpResp.GetResponseStream, Text.Encoding.GetEncoding("GB2312"))
        Dim respHTML As String = reader.ReadToEnd()
        reader.Close()
        If Not httpReq Is Nothing Then httpReq.GetResponse.Close() : httpReq = Nothing
        GetHttpHtml = respHTML
    End Function

End Class
