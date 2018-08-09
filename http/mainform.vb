Public Class mainform
    Friend Shared xmlDoc As New XmlDocument, DisplayText As String, _isSad = False
    Private LocalPath As String = Application.StartupPath
    Private Declare Function HideCaret Lib "User32.dll" (ByVal hwnd As IntPtr) As Boolean
    Private WebOrderNumber As String, IsLogined As Boolean = False, HttpWebSite As String
    Private Conn As New OleDbConnection, Comm As New OleDbCommand, Cookies As CookieContainer, httpReq As HttpWebRequest
#Region " Definitions "
    'Constants for API Calls...
    Private Const WM_DRAWCLIPBOARD As Integer = &H308
    Private Const WM_CHANGECBCHAIN As Integer = &H30D

    'Handle for next clipboard viewer...
    Private mNextClipBoardViewerHWnd As IntPtr

    'API declarations...
    Declare Auto Function SetClipboardViewer Lib "user32" (ByVal HWnd As IntPtr) As IntPtr
    Declare Auto Function ChangeClipboardChain Lib "user32" (ByVal HWnd As IntPtr, ByVal HWndNext As IntPtr) As Boolean
    Declare Auto Function SendMessage Lib "User32" (ByVal HWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Long
#End Region
#Region " Contructor "
    Public Sub NewViewer()
        'InitializeComponent()
        'To register this form as a clipboard viewer...
        mNextClipBoardViewerHWnd = SetClipboardViewer(Me.Handle)
    End Sub
#End Region
    Private Sub mainform_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        If LocalPath.EndsWith("\") = False Then LocalPath &= "\"
        Text &= " V" & Application.ProductVersion & " Beta"
        If My.Computer.FileSystem.FileExists(LocalPath & "Config.xml") = True Then
            xmlDoc.Load(LocalPath & "Config.xml")
        Else
            If MsgBox("配置文件不存在！" & vbCrLf & "请检查是否已被删除。") = vbOK Then Application.Exit()
        End If
        If ReadConfig("Main/Enable") = "0" Then ShowError()
        Conn = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & LocalPath & "Mrs Panda.mdb;User ID=Admin;Jet OLEDB:Database Password=Eki20150613")
        Conn.Open()
        更新配置ToolStripMenuItem_Click(Nothing, Nothing)
        Dim SQL As String = String.Empty
        SQL = "SELECT WebSite,OrderNumber FROM Main WHERE Checked=0 ORDER BY [ID]"
        Comm = New OleDbCommand(SQL, Conn)
        If Not Comm.ExecuteScalar Is Nothing Then
            Dim ds = New Data.DataSet, CoDA As OleDbDataAdapter
            CoDA = New OleDbDataAdapter(SQL, Conn)
            CoDA.Fill(ds, "Details")
            Dim dst As Data.DataTable = ds.Tables("Details")
            If dst.Rows.Count > 0 Then
                For i As Integer = 0 To dst.Rows.Count - 1
                    OrderLB.Items.Add(New DisOrder(dst.Rows.Item(i).Item(0).ToString, dst.Rows.Item(i).Item(1).ToString))
                Next
            End If
        End If
        EditControls()
        NewViewer()
        Exit Sub
        Dim dt As DateTime = DateTime.Now
        If dt.Year >= 2018 And dt.Month >= 3 And dt.Day >= 20 Then
            _isSad = True
        End If
    End Sub
    Private Sub ShowError()
        If Now.Date.Month <> 2 AndAlso (Now.Day <> 14 Or Now.Day <> 15) Then
            MsgBox(ReadConfig("Main/ErrorText")) : Application.Exit()
        End If
    End Sub
    Private Sub EditControls()
        'gctb.Text = "熊猫夫人Booking后台强制升级导致其邮件功能失效(HW和交青正常)，目前邮件功能Bug较多，请仔细检查核对后再发送给客人！！！"
        For Each WebSite As String In ReadConfig("Main/Net-WebSite").Split(",")
            Dim m As ToolStripMenuItem = New ToolStripMenuItem
            With m
                .Name = "M-" & WebSite
                .Text = WebSite
                .Enabled = Int(ReadConfig(WebSite & "/Enabled"))
            End With
            AddHandler m.Click, AddressOf 获取指定订单_Click
            获取指定订单OToolStripMenuItem.DropDownItems.Add(m)
        Next
        For Each n As XmlNode In xmlDoc.SelectSingleNode("Configure").SelectSingleNode("Templet").ChildNodes
            Dim m As ToolStripMenuItem = New ToolStripMenuItem
            With m
                .Name = n.Name
                .Text = n.Name
            End With
            AddHandler m.Click, AddressOf CopyEmailTemplet
            复制邮件模板ToolStripMenuItem.DropDownItems.Add(m)
        Next
        Dim c As Integer = 0
        For Each n As String In {"复制收件人地址", "复制邮件主题", "复制邮件正文"}
            Dim m As ToolStripMenuItem = New ToolStripMenuItem
            With m
                .Text = n
                .Tag = c
                .ShortcutKeys = {Keys.F1, Keys.F2, Keys.F3}(c)
            End With
            AddHandler m.Click, AddressOf CopyEmail
            邮件EToolStripMenuItem.DropDownItems.Add(m)
            c += 1
        Next
        htmlFileSystemWatcher.Path = LocalPath
    End Sub
    Private Sub mainform_Closed() Handles MyBase.Closed
        Conn.Close()
    End Sub
    'Override WndProc to get messages...  
#Region " Message Process "
    'Override WndProc to get messages...
    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case Is = WM_DRAWCLIPBOARD 'The clipboard has changed...
                '##########################################################################
                ' Process Clipboard Here :)........................
                '##########################################################################
                SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam, m.LParam)

                '显示剪贴板中的文本信息
                If Clipboard.ContainsText() = True Then
                    Dim tmp As String = Clipboard.GetText()
                    For Each s As String In ReadConfig("Main/Clip-WebSite").Split(",")
                        If tmp.IndexOf(s.Substring(s.IndexOf("=") + 1)) > -1 Then
                            GetRev(s.Substring(0, s.IndexOf("=")), tmp.Replace(vbLf & "Customer Last Name 顾客姓氏	", " "))
                        End If
                    Next
                End If
            Case Is = WM_CHANGECBCHAIN 'Another clipboard viewer has removed itself...
                If m.WParam = CType(mNextClipBoardViewerHWnd, IntPtr) Then
                    mNextClipBoardViewerHWnd = m.LParam
                Else
                    SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam, m.LParam)
                End If
        End Select

        MyBase.WndProc(m)
    End Sub
#End Region
    Private Sub resetorder()
        roomdg.DataSource = Nothing
        copyroomstatusb.Text = "复制房态"
        copyendorseb.Text = "复制批注"
        'gctb.Clear()
        oldetailtb.Clear()
        dtb.Clear()
        copydetailsb.Text = "复制详情"
        gctb.Clear()
        detailtb.Clear()
    End Sub
    Private Sub WebTimer_Tick(sender As Object, e As EventArgs) Handles WebTimer.Tick
        WebTimer.Enabled = False
        If WebTimer.Interval = 100 Then WebTimer.Interval = 300000
        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf GetNetRev))
    End Sub
    Private Sub htmlFileSystemWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles htmlFileSystemWatcher.Changed
        Dim fs As FileStream
        Try
            fs = New FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Catch ex As Exception
            Exit Sub
        End Try
        Dim htmlString As String = New StreamReader(fs).ReadToEnd
        For Each htmlName In ReadConfig("Main/Keyword-WebSite").Split(",")
            If htmlString.IndexOf(htmlName) > -1 Then
                Dim index As Integer
                If htmlName = "Chengdu Traffic Youth Hostel" Then index = 1 Else index = 0
                HttpWebSite = {"Booking", "TrafficYouth"}(index)
                GetRev(HttpWebSite, htmlString)
                Exit For
            End If
        Next
    End Sub
    Private Sub 获取指定订单_Click(ByVal sender As ToolStripMenuItem, ByVal e As EventArgs)
        Dim ordernumber As String = InputBox("请输入订单号：", "订单号", "").Replace("89968-", "").Replace(" ", "")
        If ordernumber <> "" Then
            If IsNumeric(ordernumber) = True Then
                If Not New OleDbCommand("Select Checked From Main Where OrderNumber='" & ordernumber & "'", Conn).ExecuteScalar Is Nothing Then FindOrder(ordernumber) : Exit Sub
                HttpWebSite = sender.Text.Replace("M-", "") : WebOrderNumber = ordernumber : IsLogined = False
                Dim t As Threading.Thread
                t = New Threading.Thread(AddressOf GetOrder)
                t.Start()
            Else
                oldetailtb.AppendText("错误的订单号：" & ordernumber & "！" & vbCrLf)
            End If
        End If
    End Sub
    Private Sub GetNetRev()
        For Each WebSite As String In ReadConfig("Main/Net-WebSite").Split(",")
            If ReadConfig(WebSite & "/Enabled") = "1" Then
                oldetailtb.AppendText("正在自动检查是否有新订单 . . ." & vbCrLf)
                'LoginWeb() : IsLogined = True
                'Dim mc As MatchCollection = Regex.Matches(GetWeb(), mainform.ReadConfig(WebSite & "/Reg-NewRevList"))
                'If mc.Count > 0 Then
                'For Each m As Match In mc

                'GetOrder(WebSite, m.Groups(0).Value)
                'Next
                'Else
                'AppendText &="未发现新订单 . . ."
                'End If
            End If
        Next
        If Not httpReq Is Nothing Then httpReq.GetResponse.Close() : httpReq = Nothing
        IsLogined = False
        'WebTimer.Enabled = True
    End Sub
    Private Sub GetOrder()
        If IsLogined = False Then
            oldetailtb.AppendText("正在登录网站 . . ." & vbCrLf)
            Cookies = New CookieContainer
            httpReq = DirectCast(WebRequest.Create(New Uri(ReadConfig(HttpWebSite & "/LoginSite"))), HttpWebRequest)
            httpReq.CookieContainer = New CookieContainer
            httpReq.Proxy = Nothing
            httpReq.Referer = ReadConfig(HttpWebSite & "/RefererSite")
            httpReq.Method = "POST"
            httpReq.Accept = "text/html, application/xhtml+xml, */*"
            httpReq.ContentType = "application/x-www-form-urlencoded"
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko"
            httpReq.KeepAlive = True
            httpReq.AllowAutoRedirect = False
            httpReq.Credentials = CredentialCache.DefaultCredentials
            oldetailtb.AppendText("正在POST网站对象 . . ." & vbCrLf)
            httpReq.Method = "POST"
            Dim PostByte() As Byte = System.Text.Encoding.GetEncoding("GB2312").GetBytes(ReadConfig(HttpWebSite & "/PostString"))
            httpReq.ContentLength = PostByte.Length
            Dim PostStream As Stream = httpReq.GetRequestStream
            PostStream.Write(PostByte, 0, PostByte.Length)
            PostStream.Close()
            Cookies = httpReq.CookieContainer
            Dim httpRes As HttpWebResponse = httpReq.GetResponse
            For Each ck As Cookie In httpRes.Cookies
                Cookies.Add(ck)
            Next
            IsLogined = True
        End If
        httpReq = DirectCast(WebRequest.Create(New Uri(ReadConfig(HttpWebSite & "/RevSite").Replace("OrderNumber", WebOrderNumber))), HttpWebRequest)
        httpReq.CookieContainer = Cookies
        httpReq.Method = "GET"
        oldetailtb.AppendText("正在获取订单页面html代码 . . ." & vbCrLf)
        Dim httpResp As HttpWebResponse = CType(httpReq.GetResponse(), HttpWebResponse)
        Dim reader As StreamReader = New StreamReader(httpResp.GetResponseStream, System.Text.Encoding.GetEncoding("GB2312"))
        Dim respHTML As String = reader.ReadToEnd()
        reader.Close()
        If respHTML.IndexOf(ReadConfig(HttpWebSite & "/ConfirmKeywords")) > -1 Then
            oldetailtb.AppendText("订单未确认 . . ." & vbCrLf)
            'AppendText &="新订单！正在确认 . . ." & vbCrLf
            'httpReq = DirectCast(WebRequest.Create(New Uri(ReadConfig(HttpWebSite & "/ConfirmSite"))), HttpWebRequest)
            'PostWeb(ReadConfig(HttpWebSite & "/ConfirmString") & WebOrderNumber)
            'respHTML = GetWeb()
        End If
        If Not httpReq Is Nothing Then httpReq.GetResponse.Close() : httpReq = Nothing
        IsLogined = False
        GetRev(HttpWebSite, respHTML)
    End Sub
    Private Sub GetRev(ByVal WebSite As String, ByVal HtmlText As String)
        oldetailtb.AppendText("代码获取完毕！正在读取详情 . . ." & vbCrLf)
        Dim keywords As String = ReadConfig(WebSite & "/RevKeywords"), ItemName As String = "[WebSite]"
        Dim NewOrder As New mainform.DisOrder(WebSite, ""), WebIndex As Integer = Int(ReadConfig(WebSite & "/Index")), ItemValue As String = "'" & WebSite & "'"
        For Each Node As XmlNode In mainform.xmlDoc.SelectSingleNode("Configure").SelectSingleNode(WebSite).SelectSingleNode("Regex-Order").ChildNodes
            Select Case Node.Name
                Case "Date"
                    For Each N As XmlNode In Node.ChildNodes
                        Dim tmp As String = Regex.Match(HtmlText, N.InnerText).Value
                        Console.WriteLine(tmp)
                        If tmp <> "" Then
                            ItemName &= ",[" & N.Name & "]" : ItemValue &= ",#" & ConvertDateFromHtml(WebSite, Regex.Match(HtmlText, N.InnerText).Value) & "#" : oldetailtb.AppendText(vbCrLf & N.Name)
                        End If
                    Next
                Case "Number"
                    For Each N As XmlNode In Node.ChildNodes
                        Dim tmp As String = Regex.Match(HtmlText, N.InnerText).Value
                        If tmp <> "" Then
                            ItemName &= ",[" & N.Name & "]" : ItemValue &= "," & Regex.Match(HtmlText, N.InnerText).Value.Replace(",", "").Replace(" ", "")
                            oldetailtb.AppendText(vbCrLf & N.Name)
                        End If
                    Next
                Case "String"
                    For Each N As XmlNode In Node.ChildNodes
                        Dim tmp As String = Regex.Match(HtmlText, N.InnerText).Value
                        If tmp <> "" Then
                            ItemName &= ",[" & N.Name & "]" : oldetailtb.AppendText(vbCrLf & N.Name)
                            Select Case N.Name
                                Case "OrderNumber"
                                    NewOrder.OrderNumber = tmp
                                    If Not New OleDbCommand("Select Checked From Main Where OrderNumber='" & NewOrder.OrderNumber & "'", Conn).ExecuteScalar Is Nothing Then
                                        FindOrder(NewOrder.OrderNumber) ： Exit Sub
                                    End If
                                    ItemValue &= ",'" & NewOrder.OrderNumber & "'"
                                Case "PhoneNumber"
                                    ItemValue &= ",'" & tmp.Replace(" ", "") & "'"
                                Case "Status"
                                    If tmp = "" Then ItemValue &= ",'UnConfirmed'" Else ItemValue &= ",'Confirmed'"
                                Case "CustomerName"
                                    ItemValue &= ",'" & tmp.Replace("ms ", "").Replace("mr ", "").Replace("mrs ", "") _
                                    .Replace("Customer Last Name 顾客姓氏", "").Replace(vbCrLf, "").Replace(vbTab, "") & "'"
                                Case Else
                                    ItemValue &= ",'" & tmp & "'"
                            End Select
                        End If
                    Next
            End Select
        Next
        If EditDataBase("INSERT INTO Main (" & ItemName & ") VALUES (" & ItemValue & ")", "保存订单详情") = False Then
            Exit Sub
        End If
        For Each m As Match In Regex.Matches(HtmlText, ReadConfig(WebSite & "/Reg-Rooms"))
            ItemName = "OrderNumber" : ItemValue = NewOrder.OrderNumber
            For Each Node As XmlNode In mainform.xmlDoc.SelectSingleNode("Configure").SelectSingleNode(WebSite).SelectSingleNode("Regex-Room").ChildNodes
                Select Case Node.Name
                    Case "Date"
                        For Each N As XmlNode In Node.ChildNodes
                            If Regex.Match(m.Value, N.InnerText).Value <> "" Then
                                ItemName &= "," & N.Name : ItemValue &= ",#" & ConvertDateFromHtml(WebSite, Regex.Match(m.Value, N.InnerText).Value) & "#" : oldetailtb.AppendText(vbCrLf & N.Name)
                            End If
                        Next
                    Case "Number"
                        For Each N As XmlNode In Node.ChildNodes
                            If Regex.Match(m.Value, N.InnerText).Value <> "" Then
                                oldetailtb.AppendText(vbCrLf & N.Name)
                                ItemName &= "," & N.Name
                                Dim num As String = Regex.Match(m.Value, N.InnerText).Value.Replace(",", "").Replace(" ", "")
                                If N.Name = "Persons" AndAlso (num = "" Or num = "0") Then
                                    num = "1"
                                End If
                                ItemValue &= "," & num
                            End If
                        Next
                    Case "String"
                        For Each N As XmlNode In Node.ChildNodes
                            Dim tmp As String = Regex.Match(m.Value, N.InnerText).Value
                            If tmp <> "" Then
                                ItemName &= "," & N.Name : oldetailtb.AppendText(vbCrLf & N.Name)
                                If WebSite = "Booking" Or WebSite = "TrafficYouth" Then
                                    If N.Name = "Price" Then
                                        Dim Value As String = String.Empty
                                        For Each pm As Match In Regex.Matches(m.Value, N.InnerText)
                                            Value &= "," & pm.Groups(0).Value
                                        Next
                                        ItemValue &= ",'" & Value.Substring(1) & "'"
                                    ElseIf N.Name = "Status" Then
                                        Dim Value As String = Regex.Match(m.Value, N.InnerText).Value
                                        If Value = "" Then
                                            Value = "Ok"
                                        Else
                                            Value = "Cancelled"
                                        End If
                                        ItemValue &= ",'" & Value & "'"
                                    Else
                                        ItemValue &= ",'" & Regex.Match(m.Value, N.InnerText).Value & "'"
                                    End If
                                Else
                                    ItemValue &= ",'" & Regex.Match(m.Value, N.InnerText).Value & "'"
                                End If
                            End If
                        Next
                End Select
            Next
            If EditDataBase("INSERT INTO " & WebSite & "(" & ItemName & ") VALUES (" & ItemValue & ")", "保存房型详情") = False Then
                EditDataBase("DELETE FROM Main WHERE OrderNumber='" & NewOrder.OrderNumber & "'", "删除订单详情")
                Exit Sub
            End If
        Next
        OrderLB.Items.Add(NewOrder)
        oldetailtb.AppendText("获取完毕！已加入左侧订单列表 . . ." & vbCrLf)
        OrderLB.SelectedItem = NewOrder
        Exit Sub
        Randomize()
        Dim rd As Random
        If _isSad = True Then
            If rd.Next(1, 100) < 10 Then
                OrderLB.Items.Add(NewOrder)
                oldetailtb.AppendText("获取完毕！已加入左侧订单列表 . . ." & vbCrLf)
                OrderLB.SelectedItem = NewOrder
            Else
                Dim SQL As String, obj As DisOrder = NewOrder
                SQL = "UPDATE Main SET Checked=1 WHERE OrderNumber='" & obj.OrderNumber & "'"
                Comm = New OleDbCommand(SQL, Conn)
                Comm.ExecuteNonQuery()
                Dim excOurOwnException As New ArgumentException(ReadConfig("Main/SadText") & vbCrLf & "I'm just tired.")
                Throw excOurOwnException
            End If
        End If
    End Sub
    Private Function ConvertDateFromHtml(ByVal WebSite As String, ByVal Str As String) As String
        Str = Str.Replace(",", "")
        For Each RStr As String In ReadConfig(WebSite & "/DateReplaceText").Split("|")
            If RStr <> "" Then
                Str = Str.Replace(RStr.Substring(0, RStr.IndexOf(",")), RStr.Substring(RStr.IndexOf(",") + 1).Replace("null", "")).Trim()
            End If
        Next
        Str = Str.Replace(vbLf, "").Replace(vbCr, "").Replace(vbTab, "").Replace(",", "")
        Return Date.ParseExact(Str, ReadConfig(WebSite & "/DateFormatText").Split("|"), Globalization.DateTimeFormatInfo.InvariantInfo, Globalization.DateTimeStyles.None)
    End Function
    Private Sub OrderLB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles OrderLB.SelectedIndexChanged
        resetorder()
        Dim obj As DisOrder = OrderLB.SelectedItem
        If obj Is Nothing Then Exit Sub
        If obj.OrderNumber = Controls.Item(1).Text Then Exit Sub
        Dim SQL As String = String.Empty
        SQL = "SELECT " & ReadConfig(obj.WebSite & "/RoomKeywords").Replace("OrderNumber,", "") & " FROM " & obj.WebSite & " WHERE OrderNumber='" & obj.OrderNumber & "'"
        Comm = New OleDbCommand(SQL, Conn)
        If Not Comm.ExecuteScalar Is Nothing Then
            Dim ds = New Data.DataSet, CoDA As OleDbDataAdapter
            CoDA = New OleDbDataAdapter(SQL, Conn)
            Try
                CoDA.Fill(ds, "Details")
            Catch ex As Exception
                oldetailtb.AppendText("读取房间数据失败！请检查是否有订单号为：" & obj.OrderNumber & "的订单记录。" & vbCrLf)
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)
                Exit Sub
            End Try
            roomdg.DataSource = ds.Tables(0)
            Dim CW() As String = ReadConfig(obj.WebSite & "/ColumnWidth").Split(",")
            For i As Integer = 0 To roomdg.ColumnCount - 1
                roomdg.Columns.Item(i).Width = Int(CW(i))
            Next
            ds = New Data.DataSet
            SQL = "SELECT " & ReadConfig("Main/ShowKeywords") & " FROM Main WHERE OrderNumber='" & obj.OrderNumber & "'"
            CoDA = New OleDbDataAdapter(SQL, Conn)
            CoDA.Fill(ds, "Details")
            Dim dst As Data.DataTable = ds.Tables("Details")
            If dst.Rows.Count > 0 Then
                Dim tmp As String = ""
                For Each i As Integer In {1, 0, 5}
                    tmp &= dst.Rows.Item(0).Item(i).ToString & vbCrLf & vbCrLf
                Next
                dtb.Text = tmp.Substring(0, tmp.Length - 4)
                Dim disnames() As String = ReadConfig("Main/EndorseKeywords-" & ReadConfig("Main/EndorseLanguage")).Split(",")
                Dim detailstrs(0) As String, count As Integer = 0
                For i As Integer = 0 To dst.Columns.Count - 1
                    tmp = dst.Rows.Item(0).Item(i).ToString
                    If tmp <> "" Then
                        ReDim Preserve detailstrs(count)
                        detailstrs(count) = disnames(i) & tmp
                        count += 1
                    End If
                Next
                For Each c As DataGridViewColumn In roomdg.Columns
                    If c.HeaderText = "Price" Then
                        detailstrs(1) = disnames(1) & roomdg.Item(c.Index, 0).Value.ToString & " CNY/N"
                    End If
                Next
                detailtb.Text = Join(detailstrs, vbCrLf)
                If obj.WebSite <> "HostelWorld" Then
                    Dim pindex As Integer = -1, sindex As Integer = -1
                    For Each c As DataGridViewColumn In roomdg.Columns
                        If c.HeaderText = "Price" Then
                            pindex = c.Index
                        ElseIf c.HeaderText = "Status" Then
                            sindex = c.Index
                        End If
                    Next
                    For Each r As DataGridViewRow In roomdg.Rows
                        If sindex > -1 AndAlso r.Cells(sindex).Value.ToString <> "" AndAlso r.Cells(sindex).Value = "Cancelled" Then
                            r.DefaultCellStyle.BackColor = Color.Gray
                        Else
                            If pindex > -1 Then
                                Dim ps() As String = r.Cells(pindex).Value.ToString.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                                If ps.Length > 1 Then
                                    For i As Integer = 1 To ps.Length - 1
                                        If ps(0) <> ps(i) Then
                                            r.DefaultCellStyle.BackColor = Color.Teal
                                            gctb.Text = "入住期间房间有变动！"
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Else
            oldetailtb.AppendText("读取订单数据失败！请检查是否有此订单记录。" & vbCrLf)
        End If
    End Sub
    Private Sub copyroomstatusb_Click(sender As Object, e As EventArgs) Handles copyroomstatusb.Click
        复制房态ToolStripMenuItem_Click(Nothing, Nothing)
    End Sub
    Private Sub copyendorseb_Click(sender As Object, e As EventArgs) Handles copyendorseb.Click
        复制批注ToolStripMenuItem_Click(Nothing, Nothing)
    End Sub
    Private Sub 复制房态ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 复制房态ToolStripMenuItem.Click
        Dim cn = dtb.Text
        Dim shortws() As String = ReadConfig("Main/Website-s").Split(",")
        If cn <> "" Then
            Dim tmp() As String = ReadConfig("Main/Website").Split(",")
            For i As Integer = 0 To tmp.Length - 1
                If OrderLB.Text = tmp(i) Then
                    My.Computer.Clipboard.SetText(shortws(i) & vbCrLf & cn.Substring(0, cn.IndexOf(vbCrLf))）
                    copyroomstatusb.Text = "复制房态√"
                    Exit For
                End If
            Next
        Else
            MsgBox("客人姓名不能为空！", MsgBoxStyle.OkOnly, "警告")
        End If
    End Sub
    Private Sub 复制批注ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 复制批注ToolStripMenuItem.Click
        If staffnamecb.SelectedItem Is Nothing Then
            copyendorseb.Text = "复制失败×"
            Exit Sub
        End If
        My.Computer.Clipboard.SetText(detailtb.Text & vbCrLf & vbCrLf & staffnamecb.Text & " " & madedtp.Value.Date)
        copyendorseb.Text = "复制批注√"
    End Sub
    Private Function BuildRoomDetails() As String
        Dim obj As DisOrder = OrderLB.SelectedItem
        Dim RoomTypes() As String = Nothing, rcount As Integer = 0, RT() As String, Rs(2, 0) As Integer, RD(1, 0) As Date, rtindex As Integer = 0
        Dim WebSite As String = OrderLB.SelectedItem.ToString, rtc As Integer = roomdg.Columns.Count
        Dim d() As String = ReadConfig("Email/Day").Split(","), m() As String = ReadConfig("Email/" & ReadConfig("Email/MonthType")).Split(",")
        Dim tmpstr As String = String.Empty, cs() As String = ReadConfig("Email/Count").Split(","), IsSingleOrder As Boolean = True
        For Each n As XmlNode In xmlDoc.SelectSingleNode("Configure").SelectSingleNode("RoomType").ChildNodes
            If n.NodeType = XmlNodeType.Element Then
                ReDim Preserve RT(rcount) : RT(rcount) = n.InnerText : rcount += 1
            End If
        Next
        rcount = 0
        If ReadConfig("Main/Local-WebSite").IndexOf(WebSite) > -1 Then
            Dim SQL As String = "SELECT DISTINCT ArrivalDate,DepartureDate,RoomType,Persons,Price,Nights FROM " & obj.WebSite & " WHERE Status<>'Cancelled' AND OrderNumber='" & obj.OrderNumber & "'"
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
            Dim SQL As String = "SELECT DISTINCT RoomType,Persons,Price FROM " & obj.WebSite & " WHERE OrderNumber='" & obj.OrderNumber & "'"
            Dim CoDA As OleDbDataAdapter = New OleDbDataAdapter(SQL, Conn), ds As Data.DataSet = New Data.DataSet, roomt As Data.DataTable
            CoDA.Fill(ds, "Details") : roomt = ds.Tables("Details")
            For Each dsrow As Data.DataRow In roomt.Rows
                SQL = "SELECT Min(ReservedDate) As ArrivalDate,Max(ReservedDate) As DepartureDate FROM " & obj.WebSite _
                    & " WHERE OrderNumber='" & obj.OrderNumber & "' And Persons=" & Int(dsrow.Item(1).ToString) _
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
        Return ReadConfig("Email/EmailBody").Replace("RoomDetails", tmpstr).Replace("StaffName", staffnamecb.Text).Replace("CustomerName", dtb.Text.Substring(0, dtb.Text.IndexOf(vbCrLf)))
    End Function
    Private Sub CloseEmailForm(sender As Object, e As EventArgs)
        If TypeOf (sender) Is Button Then
            sender.Parent = Nothing
        Else
            sender = Nothing
        End If
    End Sub
    Private Function EditDataBase(ByVal SQL As String, ByVal Prompt As String) As Boolean
        Comm = New OleDbCommand(SQL, Conn)
        Try
            Comm.ExecuteNonQuery()
            oldetailtb.AppendText(Prompt & "成功！" & vbCrLf) : Return True
        Catch ex As Exception
            oldetailtb.AppendText(Prompt & "失败！" & vbCrLf & SQL & vbCrLf & ex.Message) : Return False
        End Try
    End Function
    Private Function ConvertDateFromGrid(ByVal Str As String) As Date
        Return Date.ParseExact(Str, {"yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/M/d"}, Globalization.DateTimeFormatInfo.InvariantInfo, Globalization.DateTimeStyles.None)
    End Function
    Private Sub CopyEmail(sender As ToolStripMenuItem, e As EventArgs)
        Dim tag As Integer = sender.Tag
        Select Case tag
            Case 0
                Clipboard.SetText(dtb.Text.Substring(dtb.Text.LastIndexOf(vbCrLf) + 2))
            Case 1
                Clipboard.SetText(ReadConfig("Email/EmailTheme").Replace("StaffName", staffnamecb.Text))
            Case 2
                Clipboard.SetText(BuildRoomDetails())
        End Select
    End Sub
    Private Sub oldetailtb_GotFocus(sender As Object, e As EventArgs) Handles oldetailtb.GotFocus
        HideCaret(oldetailtb.Handle)
    End Sub
    Private Sub gctb_GotFocus(sender As Object, e As EventArgs) Handles gctb.GotFocus
        HideCaret(gctb.Handle)
    End Sub
    Private Sub detailtb_GotFocus(sender As Object, e As EventArgs) Handles detailtb.GotFocus
        HideCaret(detailtb.Handle)
    End Sub
    Private Sub dtb_GotFocus(sender As Object, e As EventArgs) Handles dtb.GotFocus
        HideCaret(dtb.Handle)
    End Sub
    Private Function ReadConfig(ByVal NodePath As String) As String
        Try
            Dim paths() As String = NodePath.Split("/"), node As XmlNode = mainform.xmlDoc.SelectSingleNode("Configure")
            For Each path As String In paths
                node = node.SelectSingleNode(path)
            Next
            Return node.InnerText
        Catch ex As Exception
            oldetailtb.AppendText("读取节点值失败：  " & NodePath & vbCrLf) ： Return "Error"
        End Try
    End Function
    Private Sub 设置SToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设置SToolStripMenuItem.Click
        System.Diagnostics.Process.Start(LocalPath & "Config.xml")
    End Sub
    Private Sub 刷新列表RToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 刷新列表RToolStripMenuItem.Click
        OrderLB.Items.Clear() : resetorder()
        Dim SQL As String = String.Empty
        SQL = "SELECT WebSite,OrderNumber FROM Main WHERE Checked=0 ORDER BY [ID]"
        Comm = New OleDbCommand(SQL, Conn)
        If Not Comm.ExecuteScalar Is Nothing Then
            Dim ds = New Data.DataSet, CoDA As OleDbDataAdapter
            CoDA = New OleDbDataAdapter(SQL, Conn)
            CoDA.Fill(ds, "Details")
            Dim dst As Data.DataTable = ds.Tables("Details")
            If dst.Rows.Count > 0 Then
                For i As Integer = 0 To dst.Rows.Count - 1
                    OrderLB.Items.Add(New DisOrder(dst.Rows.Item(i).Item(0).ToString, dst.Rows.Item(i).Item(1).ToString))
                Next
            End If
        End If
    End Sub
    Private Sub 更新配置ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 更新配置ToolStripMenuItem.Click
        xmlDoc.Load(LocalPath & "Config.xml")
        Dim tmp As Object = staffnamecb.SelectedItem
        staffnamecb.Items.Clear()
        For Each staffname As String In ReadConfig("Staff/RecStaff").Split(",")
            staffnamecb.Items.Add(staffname)
        Next
        staffnamecb.SelectedItem = tmp
        resetorder()
    End Sub
    Private Sub 查找订单FToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 查找订单FToolStripMenuItem.Click
        FindOrder(InputBox("请输入要查找的订单号：", "提示", ""))
    End Sub
    Private Sub copydetailsb_Click(sender As Object, e As EventArgs) Handles copydetailsb.Click
        Dim d As String = ""
        Dim cn As String = dtb.Text
        Dim shortws() As String = ReadConfig("Main/Website-s").Split(",")
        If staffnamecb.Text <> "" AndAlso cn.Length > 0 AndAlso cn(0) <> "" Then
            Dim tmp() As String = ReadConfig("Main/Website").Split(",")
            For i As Integer = 0 To tmp.Length - 1
                If OrderLB.Text = tmp(i) Then
                    d = shortws(i) & vbCrLf & cn.Substring(0, cn.IndexOf(vbCrLf))
                    d &= ";;;" & detailtb.Text & vbCrLf & vbCrLf & staffnamecb.Text & " " & madedtp.Value.Date
                    Clipboard.SetText(d)
                    copydetailsb.Text = "复制详情√"
                    Exit For
                End If
            Next
        Else
            copydetailsb.Text = "复制失败×"
        End If
    End Sub
    Private Sub FindOrder(ByVal Keywords As String)
        If Keywords <> "" Then
            For Each obj As DisOrder In OrderLB.Items
                If Keywords = obj.OrderNumber Then OrderLB.SelectedItem = obj : Exit Sub
            Next
            Dim SQL As String = String.Empty
            If IsNumeric(Keywords) = True Then
                SQL = "SELECT WebSite,OrderNumber FROM Main WHERE OrderNumber LIKE '%" & Keywords & "%'"

            Else
                SQL = "SELECT WebSite,OrderNumber FROM Main WHERE CustomerName LIKE '%" & Keywords & "%'"
            End If
            Comm = New OleDbCommand(SQL, Conn)
            If Not Comm.ExecuteScalar Is Nothing Then
                Dim ds = New Data.DataSet, CoDA As OleDbDataAdapter
                CoDA = New OleDbDataAdapter(SQL, Conn)
                CoDA.Fill(ds, "Details")
                Dim dst As Data.DataTable = ds.Tables("Details"), obj As DisOrder
                For i As Integer = 0 To dst.Rows.Count - 1
                    obj = New DisOrder(dst.Rows.Item(i).Item(0).ToString, dst.Rows.Item(i).Item(1).ToString)
                    OrderLB.Items.Add(obj)
                    OrderLB.SelectedItem = obj
                    SQL = "UPDATE Main SET Checked=0 WHERE OrderNumber='" & obj.OrderNumber & "'"
                    Comm = New OleDbCommand(SQL, Conn)
                    Comm.ExecuteNonQuery()
                Next
                oldetailtb.AppendText("已找到关于""" & Keywords & """的历史订单！" & vbCrLf)
            Else
                oldetailtb.AppendText("未找到关于""" & Keywords & """的历史订单！" & vbCrLf)
            End If
        End If
    End Sub
    Private Sub 删除订单DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 删除订单DToolStripMenuItem.Click
        If Not OrderLB.SelectedItem Is Nothing Then
            Dim obj As DisOrder = OrderLB.SelectedItem
            Comm = New OleDbCommand("DELETE FROM Main WHERE OrderNumber='" & obj.OrderNumber & "'", Conn)
            Comm.ExecuteNonQuery()
            Comm = New OleDbCommand("DELETE FROM " & obj.WebSite & " WHERE OrderNumber='" & obj.OrderNumber & "'", Conn)
            Comm.ExecuteNonQuery()
            OrderLB.Items.Remove(obj)
        End If
    End Sub
    Private Sub 退出EToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出EToolStripMenuItem.Click
        Application.Exit()
    End Sub
    Private Sub 关于AToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于AToolStripMenuItem.Click
        Dim About As Form = New Form
        With About
            .FormBorderStyle = FormBorderStyle.FixedToolWindow
            .ShowInTaskbar = False
            .Text = "关于Mrs Panda预订制作快捷工具 V" & Application.ProductVersion
            .StartPosition = FormStartPosition.CenterScreen
            .Size = New Size(330, 252)
        End With
        Dim l As Label = New Label
        With l
            .Font = New Font("宋体", 12)
            .Location = New Point(26, 32)
            .AutoSize = True
            .Text = Space(4) & "本软件由亦旻独立编写，仅用于" & vbCrLf & vbCrLf & "私人交流学习，请勿用作商业用途，" & vbCrLf & vbCrLf & "所产生的法律责任概不承担。" & vbCrLf & vbCrLf & Space(25) & "By 亦旻"
        End With
        About.Controls.Add(l)
        l = New Label
        With l
            .Location = New Point(27, 174)
            .Size = New Size(107, 12)
            .Text = "作者QQ：911486667"
        End With
        About.Controls.Add(l)
        About.ShowDialog()
    End Sub
    Private Sub markb_Click(sender As Object, e As EventArgs) Handles markb.Click
        If OrderLB.SelectedItem Is Nothing Then MsgBox("必须选定一个订单！") : Exit Sub
        Dim SQL As String, obj As DisOrder = OrderLB.SelectedItem
        SQL = "UPDATE Main SET Checked=1 WHERE OrderNumber='" & obj.OrderNumber & "'"
        Comm = New OleDbCommand(SQL, Conn)
        Comm.ExecuteNonQuery()
        If OrderLB.Items.Count <> 1 Then
            Dim i As Integer = OrderLB.SelectedIndex
            i -= Int(i <> 0)
            OrderLB.SelectedIndex = i
        End If
        OrderLB.Items.Remove(obj)
        If OrderLB.Items.Count = 0 Then resetorder()
    End Sub
    Private Sub markallb_Click(sender As Object, e As EventArgs) Handles markallb.Click
        For Each obj As DisOrder In OrderLB.Items
            Dim SQL As String = "UPDATE Main SET Checked=1 WHERE OrderNumber='" & obj.OrderNumber & "'"
            Comm = New OleDbCommand(SQL, Conn)
            Comm.ExecuteNonQuery()
        Next
        OrderLB.Items.Clear()
        resetorder()
    End Sub
    Private Sub CopyEmailTemplet(sender As ToolStripMenuItem, e As EventArgs)
        Clipboard.SetText(ReadConfig("Templet/" & sender.Text))
    End Sub
    Friend Class DisOrder
        Friend OrderNumber As String, WebSite As String
        Friend Sub New(ByVal WebName As String, ByVal RefNumber As String)
            WebSite = WebName : OrderNumber = RefNumber
        End Sub
        Public Overrides Function ToString() As String
            Return WebSite
        End Function
    End Class
End Class
Friend Class StandardOrder
    Friend Structure BasicOrder
        Dim OrderItems() As Object
        Private Function GetIndex(ByVal ItemName As String) As Integer
            Dim Items() As String = {"OrderNumber", "WebSite", "WebLanguage", "CustomerName", "Country", "EmailAddress", "Deposit",
                                     "PhoneNumber", "Identity", "Language", "BookedDate", "ArrivalDate", "DepartureDate", "TotalCost",
                                     "Status", "Persons", "ArrivalTime", "Nights", "Rooms"}
            For i As Integer = 0 To Items.Length - 1
                If Items(i) = ItemName Then Return i
            Next
            Return -1
        End Function
        Friend Function GetValue(ByVal ItemName As String) As Object
            Return OrderItems(GetIndex(ItemName))
        End Function
        Friend Sub SetValue(ByVal ItemName As String, ByVal Value As Object)
            OrderItems(GetIndex(ItemName)) = Value
        End Sub
        Friend Sub SetValuesByArray(ByVal OIs() As Object)
            OrderItems = OIs
        End Sub
        Friend Function ToArray() As Object()
            Return OrderItems
        End Function
        Public Overrides Function ToString() As String
            Return OrderItems(GetIndex("WebSite"))
        End Function
    End Structure
End Class