Imports SweatWorksWCF.DataContracts
Imports SweatWorksData
Imports SweatWorksData.ErrorLogging

Public Class Service1
    Implements IService1

    Private intAppID As Integer = 2
    Public Sub New()
    End Sub

#Region "Users and Authentication"

    Public Function Login(ByVal thisRequest As dcLoginRequest) As dcUserResponse Implements IService1.Login
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim strEmail = thisRequest.Email
        Dim strPassword = thisRequest.Password

        'validate
        If thisRequest.Email = Nothing Or (thisRequest.Password = Nothing And thisRequest.FacebookID = Nothing) Then
            LogError("LoginV1 validation Error, thisRequest: " + thisRequest.toString, intAppID, Now)
            status = Net.HttpStatusCode.BadRequest
            Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "You didn't supply the correct parameters.", .Successful = False}}
        End If

        Try
            Using dbGet As New SweatWorksEntities
                Dim thisUser As User = (From u In dbGet.Users Where u.Email = strEmail).FirstOrDefault

                If Not thisUser Is Nothing Then

                    If Not thisUser.PasswordHash Is Nothing And Not thisUser.PasswordHash = "none" Then
                        If HashTools.ValidatePassword(strPassword, thisUser.PasswordHash) Then
                            Try
                                'log user activity
                                dbGet.UserActivitys.Add(New UserActivity With {.AppID = intAppID, .UserID = thisUser.UserID, .DateRecorded = Now, .UserActivityTypeID = 1})
                                dbGet.SaveChanges()
                            Catch ex As Exception
                                LogError(ex, "Login Error: error saving user activity during login.", intAppID, Now)
                            End Try
                            'success
                            Return New dcUserResponse(thisUser)
                        Else
                            status = Net.HttpStatusCode.BadRequest
                            LogError("Login Error: wrong password.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                            Return New dcUserResponse With {.Status = New dcOperationStatus With {.Notes = "wrong password", .ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Successful = False}}
                        End If
                    Else
                        status = Net.HttpStatusCode.BadRequest
                        LogError("Login Error: user has no password.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                        Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Notes = "user has no password", .Successful = False}}
                    End If

                Else
                    'no user found
                    status = Net.HttpStatusCode.BadRequest
                    LogError("Login Error: user doesn't exist/failed login.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                    Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Successful = False}}
                End If
            End Using
        Catch ex As Exception
            LogError(ex, "Login Error", intAppID, Now)
            status = Net.HttpStatusCode.Conflict
            Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "There was an error logging you in, please try again later.", .ExMessage = ex.ToString, .Successful = False}}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try
    End Function

    Public Function Register(ByVal thisRequest As dcRegistrationRequest) As dcUserResponse Implements IService1.Register
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisUser As User = Nothing
        Dim userResponse As dcUserResponse = Nothing
        Dim boolCheckedConsent As Boolean = Nothing
        Dim boolOptedIn As Boolean = Nothing
        Dim boolAcceptedTerms As Boolean = Nothing

        Try
            'VALIDATE
            If Not helpers.Strings.IsValidEmail(thisRequest.Email) Then
                LogError("Registration Error, invalid email.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, invalid email.", .Successful = False}}
            ElseIf Not helpers.Strings.IsValidPassword(thisRequest.Password) Then
                LogError("Registration Error, invalid password.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, invalid password.", .Successful = False}}
            ElseIf thisRequest.FirstName = Nothing Or thisRequest.LastName = Nothing Then
                LogError("Registration Error, either a name or firstName/lastName combination is required.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, name required.", .Successful = False}}
            End If
            'If Not thisRequest.CheckedTerms = Nothing Then
            '    Try
            '        boolAcceptedTerms = CBool(thisRequest.CheckedTerms)
            '    Catch ex As Exception
            '        LogError("Registration Error, Checked terms was provided, but was not convertable to type boolean." + thisRequest.toString, intAppID, Now)
            '        status = Net.HttpStatusCode.BadRequest
            '        Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, Checked terms was provided, but was not convertable to type boolean.", .Successful = False}}
            '    End Try
            'End If
            'If Not thisRequest.CheckedConsent = Nothing Then
            '    Try
            '        boolCheckedConsent = CBool(thisRequest.CheckedConsent)
            '    Catch ex As Exception
            '        LogError("Registration Error, Consent terms was provided, but was not convertable to type boolean." + thisRequest.toString, intAppID, Now)
            '        status = Net.HttpStatusCode.BadRequest
            '        Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, Checked Consent was provided, but was not convertable to type boolean.", .Successful = False}}
            '    End Try
            'End If


            Using db As New SweatWorksEntities
               
                thisUser = (From u In db.Users Where u.Email = thisRequest.Email).FirstOrDefault
                If Not thisUser Is Nothing Then
                    status = Net.HttpStatusCode.BadRequest
                    LogError("Registration Error: user already exists", intAppID, Now)
                    Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error: user already exists with email: " + thisUser.Email, .Successful = False}}
                Else
                    Dim newUser As New User With {.FirstName = thisRequest.FirstName, .LastName = thisRequest.LastName, .Email = thisRequest.Email, .DateCreated = Now, .UserTypeID = 1} 'usertype: user
                    newUser.PasswordHash = HashTools.CreateHash(thisRequest.Password)
                    db.Users.Add(newUser)
                    userResponse = New dcUserResponse(newUser)
                End If

            End Using

            status = Net.HttpStatusCode.Created
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "Registration Error", intAppID, Now)
            Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during Registration.", .ExMessage = ex.ToString, .Successful = False}}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

        Return userResponse
    End Function

#End Region


End Class
