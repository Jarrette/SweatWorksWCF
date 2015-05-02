Imports System.IO
Imports System.Net.Mail
Imports SweatWorksData
'Imports System.IdentityModel.Selectors
'Imports System.IdentityModel.Tokens
Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceModel.Channels
Imports System.Drawing.Imaging


Namespace Helpers

    Public Class Images


        Public Shared Function ImageToStream(ByVal thisImage As System.Drawing.Image, ByVal thisImageFormat As ImageFormat)
            Dim thisStream As New System.IO.MemoryStream()
            thisImage.Save(thisStream, thisImageFormat)
            thisStream.Position = 0
            Return thisStream
        End Function


    End Class

    Public Class Errors


        Public Shared Sub LogError(ByVal exc As Exception, ByVal strNotes As String, ByVal intAppID As Integer, ByVal dateLogged As Date)
            Using dbError As New SweatWorksEntities
                Dim thisError As New AppError With {.notes = strNotes, .AppID = intAppID, .DateCreated = dateLogged}
                If Not exc Is Nothing Then
                    thisError.ex_message = exc.Message
                    If Not exc.InnerException Is Nothing Then
                        thisError.ex_inner_exception = exc.InnerException.Message
                    End If
                End If
                Try
                    dbError.AppErrors.Add(thisError)
                    dbError.SaveChanges()
                Catch ex As Exception
                    Dim strMessage As String = ex.Message
                End Try
            End Using
        End Sub

        Public Shared Sub LogError(ByVal strNotes As String, ByVal intAppID As Integer, ByVal dateLogged As Date)
            Using dbError As New SweatWorksEntities
                Dim thisError As New AppError With {.notes = strNotes, .AppID = intAppID, .DateCreated = dateLogged}
                Try
                    dbError.AppErrors.Add(thisError)
                    dbError.SaveChanges()
                Catch ex As Exception
                    Dim strMessage As String = ex.Message
                End Try
            End Using
        End Sub


    End Class

    Public Class Notify

        Public Shared Sub SendEmail(ByVal strSmtpClient As String, ByVal strUsername As String, ByVal strPassword As String, ByVal strFrom As String, ByVal strTo As String, ByVal strSubject As String, ByVal sbMessage As String)
            Try
                'create the mail message
                Dim mail As New MailMessage()
                mail.IsBodyHtml = True

                'set the addresses
                mail.From = New MailAddress(strFrom)
                mail.To.Add(strTo)

                'set the content
                mail.Subject = strSubject
                mail.Body = sbMessage
                'mail.Attachments.Add(New Attachment(strAttachmentURL))

                'set the SMTP object
                'Use ports 25 or 587 for plain/TLS connections and port 465 for SSL connections
                Dim smtp As New SmtpClient(strSmtpClient, Convert.ToInt32(587))
                smtp.Credentials = New System.Net.NetworkCredential(strUsername, strPassword)

                'send the message
                smtp.Send(mail)
            Catch ex As Exception
                Errors.LogError(ex, "FAILED MESSAGE: " + strSubject + "<br><br>" + sbMessage.ToString, 3, Now)
                Throw (ex)
            End Try
        End Sub

        Public Shared Function SendEmailWithDataTable(ByVal strSmtpClient As String, ByVal strFrom As String, ByVal strTo As String, ByVal strSubject As String, ByVal dt As DataTable, ByVal sbMessage As String) As Exception
            Dim mail As New MailMessage()

            'create the mail message

            mail.IsBodyHtml = True

            'set the addresses
            mail.From = New MailAddress(strFrom)
            mail.To.Add(strTo)

            'set the content
            mail.Subject = strSubject
            mail.Body = sbMessage + "<br><br>" + ConvertDataTableToHTML(dt)
            'mail.Attachments.Add(New Attachment(strAttachmentURL))

            'set the SMTP object
            Dim smtp As New SmtpClient(strSmtpClient)
            'smtp.Credentials = New System.Net.NetworkCredential(txtUsername.Text, txtPassword.Text, txtDomain.Text)
            'smtp.UseDefaultCredentials = False

            Try
                'send the message
                smtp.Send(mail)
                Return Nothing
            Catch ex As Exception
                Errors.LogError(ex, "Body of Email: " + mail.Body, 1, Now)
                Return ex
            End Try
        End Function

        Protected Shared Function ConvertDataTableToHTML(ByVal dt As DataTable) As String
            Dim sb As New StringBuilder
            Dim intColumnCount As Integer = dt.Columns.Count

            sb.Append("<html><head></head><body>")

            Using sw As StringWriter = New StringWriter()
                Using htw As New HtmlTextWriter(sw)

                    ' Create a form to contain the grid
                    Dim Table As New HtmlTable()
                    Dim tHead As New HtmlTableRow


                    'Add the header
                    For i = 0 To intColumnCount - 1
                        Dim tHeadCell As New HtmlTableCell
                        tHeadCell.InnerHtml = dt.Columns(i).ColumnName
                        tHead.Cells.Add(tHeadCell)
                    Next
                    Table.Rows.Add(tHead)

                    ' add each data rows as rows to the table
                    For Each dr As DataRow In dt.Rows
                        Dim tRow As New HtmlTableRow()
                        For i = 0 To intColumnCount - 1
                            Dim tCell As New HtmlTableCell()
                            tCell.InnerHtml = dr(i)
                            tCell.Style.Add("padding", "5px")
                            tRow.Cells.Add(tCell)
                        Next
                        Table.Rows.Add(tRow)
                    Next

                    '  render the table into the htmlwriter
                    Table.RenderControl(htw)

                    '  render the htmlwriter into the response
                    sb.Append(sw.ToString())
                End Using
            End Using

            'End the HTML document
            sb.Append("</body></html>")
            Return sb.ToString
        End Function
    End Class

    'Public Class MyX509CertificateValidator
    '    Inherits X509CertificateValidator
    '    Private allowedIssuerName As String

    '    Public Sub New(ByVal allowedIssuerName As String)
    '        If allowedIssuerName Is Nothing Then
    '            Throw New ArgumentNullException("allowedIssuerName")
    '        End If

    '        Me.allowedIssuerName = allowedIssuerName

    '    End Sub

    '    Public Overrides Sub Validate(ByVal certificate As X509Certificate2)
    '        ' Check that there is a certificate.
    '        If certificate Is Nothing Then
    '            Throw New ArgumentNullException("certificate")
    '        End If

    '        ' Check that the certificate issuer matches the configured issuer.
    '        If allowedIssuerName <> certificate.IssuerName.Name Then
    '            Throw New SecurityTokenValidationException _
    '              ("Certificate was not issued by a trusted issuer")
    '        End If

    '    End Sub
    'End Class

    Public Class Strings

        ''' <summary>
        ''' if a non numeric is entered, 0 is returned
        ''' </summary>
        ''' <param name="hf"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSafeHiddenInteger(ByVal hf As HiddenField) As Integer
            If IsNumeric(hf.Value) Then
                Return CInt(hf.Value)
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' if a non numeric is entered, 0D is returned
        ''' </summary>
        ''' <param name="TextBoxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSafeTextBoxDecimal(ByVal TextBoxValue As String) As Double
            If IsNumeric(TextBoxValue) Then
                Return Convert.ToDouble(TextBoxValue)
            Else
                Return 0D
            End If
        End Function

        ''' <summary>
        ''' if it's a date, it is returned as a date, otherwise NOTHING is returned
        ''' </summary>
        ''' <param name="TextBoxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSafeTextBoxDate(ByVal TextBoxValue As String) As Date
            If IsDate(TextBoxValue) Then
                Return CDate(TextBoxValue)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' if no valid integer is selected in listitem VALUE, 0 is returned
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSafeDropDownInt(ByVal ddl As DropDownList) As Integer
            If IsNumeric(ddl.SelectedValue) Then
                Return CInt(ddl.SelectedValue)
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' takes a string and gets rid of stuff that can't be shown through the querystring URI
        ''' </summary>
        ''' <param name="strText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSafeQuerystring(ByVal strText As String) As String
            strText = HttpContext.Current.Server.UrlEncode(strText)
            Return strText
        End Function

        ''' <summary>
        ''' safely converts a string into a corresponding boolean value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function StringToBoolean(ByVal thisString As String) As Boolean
            Select Case Trim(thisString.ToLower)
                Case ""
                    Return False
                Case "0"
                    Return False
                Case "1"
                    Return True
                Case "true"
                    Return True
                Case "false"
                    Return False
                Case Else
                    Return False
            End Select
        End Function

        Public Shared Function IsValidEmail(ByVal strEmail As String) As Boolean
            Dim pattern As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"
            Dim emailAddressMatch As Match = Regex.Match(strEmail, pattern)
            If emailAddressMatch.Success Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function IsValidPassword(ByVal strpassword As String) As Boolean
            If Not strpassword.Length > 7 Then
                Return False
            End If
            If Not Regex.IsMatch(strpassword, "\d") Or Not Regex.IsMatch(strpassword, "[a-zA-Z]") Then
                Return False
            End If
            Return True
        End Function

        Public Shared Function BasicFormattingToHTML(ByVal strText As String) As String
            Dim temp As String = strText
            temp = temp.Replace(Environment.NewLine, "<br>")
            Return temp
        End Function

        Public Shared Function HTMLToBasicFormatting(ByVal strText As String) As String
            Dim temp As String = strText
            temp = temp.Replace("&nbsp;", " ")
            temp = temp.Replace("<br>", Environment.NewLine)
            Return temp
        End Function

        Public Shared Function Base64ToImage(ByVal base64string As String) As System.Drawing.Image
            'Setup image and get data stream together
            Dim img As System.Drawing.Image
            Dim MS As System.IO.MemoryStream = New System.IO.MemoryStream
            Dim b64 As String = base64string.Replace(" ", "+")
            Dim b() As Byte

            'Converts the base64 encoded msg to image data
            b = Convert.FromBase64String(b64)
            MS = New System.IO.MemoryStream(b)

            'creates image
            img = System.Drawing.Image.FromStream(MS)

            Return img
        End Function
    End Class

    Public Class JsonContentTypeMapper
        Inherits WebContentTypeMapper

        Public Overrides Function GetMessageFormatForContentType(ByVal contentType As String) As WebContentFormat
            If contentType = "text/plain; charset=utf-8" Then
                Return WebContentFormat.Json
            Else
                Return WebContentFormat.Default
            End If
        End Function

    End Class

End Namespace
