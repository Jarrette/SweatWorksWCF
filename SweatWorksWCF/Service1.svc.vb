Imports SweatWorksWCF.DataContracts
Imports SweatWorksData
Imports SweatWorksData.ErrorLogging
Imports System.Data.Entity.Core.Objects
Imports SweatWorksWCF.Helpers

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
            LogError("Login validation Error, thisRequest: " + thisRequest.toString, intAppID, Now)
            status = Net.HttpStatusCode.BadRequest
            Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "You didn't supply the correct parameters.", .Successful = False}}
        End If

        Try
            Using db As New SweatWorksEntities
                Dim thisUser As User = (From u In db.Users Where u.Email = strEmail).FirstOrDefault

                If Not thisUser Is Nothing Then
                    'USER EXISTS
                    If Not thisUser.PasswordHash Is Nothing And Not thisUser.PasswordHash = "none" Then
                        If HashTools.ValidatePassword(strPassword, thisUser.PasswordHash) Then
                            Try
                                'log user activity
                                db.UserActivitys.Add(New UserActivity With {.AppID = intAppID, .UserID = thisUser.UserID, .DateRecorded = Now, .UserActivityTypeID = 1})
                                db.SaveChanges()
                            Catch ex As Exception
                                LogError(ex, "Login Error: error saving user activity during login.", intAppID, Now)
                            End Try
                            'success
                            Dim thisDCUser As New dcUserResponse(thisUser)
                            If Not thisUser.Company Is Nothing Then
                                thisDCUser.CompanyName = thisUser.Company.CompanyName
                            End If
                            Return thisDCUser
                        Else
                            status = Net.HttpStatusCode.BadRequest
                            LogError("Login Error: wrong password.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                            Return New dcUserResponse With {.Status = New dcOperationStatus With {.Notes = "wrong password", .ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Successful = False}}
                        End If
                    ElseIf thisRequest.FacebookID = thisUser.FacebookID Then
                        'SUCCESSFUL FACEBOOK LOGIN (existing user)
                        'log user activity
                        db.UserActivitys.Add(New UserActivity With {.AppID = intAppID, .UserID = thisUser.UserID, .DateRecorded = Now, .UserActivityTypeID = 1})
                        db.SaveChanges()
                        'success
                        Dim thisDCUser As New dcUserResponse(thisUser)
                        If Not thisUser.Company Is Nothing Then
                            thisDCUser.CompanyName = thisUser.Company.CompanyName
                        End If
                        Return thisDCUser
                    Else
                        status = Net.HttpStatusCode.BadRequest
                        LogError("Login Error: user has no password.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                        Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Notes = "user has no password", .Successful = False}}
                    End If
                Else
                    'USER DOESNT EXIST
                    If Not thisRequest.FacebookID = Nothing And Not thisRequest.FirstName = Nothing And Not thisRequest.LastName = Nothing Then
                        'FACEBOOK LOGIN (Create new user)
                        Dim newUser As New User With {.FirstName = thisRequest.FirstName, .LastName = thisRequest.LastName, .Email = thisRequest.Email, .FacebookID = thisRequest.FacebookID, .DateCreated = Now, .UserTypeID = 1}
                        db.Users.Add(newUser)
                        db.SaveChanges()
                        status = Net.HttpStatusCode.Created
                        Return New dcUserResponse(newUser)
                    Else
                        'INSUFFICIENT FACEBOOK CREDENTIALS
                        status = Net.HttpStatusCode.BadRequest
                        LogError("Login Error: user doesn't exist/failed login.  strEmail: " + strEmail + " strPassword: " + strPassword, intAppID, Now)
                        Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "There is not a user account associated with this email and/or password.", .Successful = False}}
                    End If
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
        Dim sbStatusMessage As New StringBuilder

        Try
            'VALIDATE
            If Not Helpers.Strings.IsValidEmail(thisRequest.Email) Then
                LogError("Registration Error, invalid email.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, invalid email.", .Successful = False}}
            ElseIf Not Helpers.Strings.IsValidPassword(thisRequest.Password) Then
                LogError("Registration Error, invalid password.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, invalid password.", .Successful = False}}
            ElseIf thisRequest.FirstName = Nothing Or thisRequest.LastName = Nothing Then
                LogError("Registration Error, either a name or firstName/lastName combination is required.  " + thisRequest.toString, intAppID, Now)
                status = Net.HttpStatusCode.BadRequest
                Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error, name required.", .Successful = False}}
            End If

            Using db As New SweatWorksEntities

                thisUser = (From u In db.Users Where u.Email = thisRequest.Email).FirstOrDefault
                If Not thisUser Is Nothing Then
                    status = Net.HttpStatusCode.BadRequest
                    LogError("Registration Error: user already exists", intAppID, Now)
                    Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Registration Error: user already exists with email: " + thisUser.Email, .Successful = False}}
                Else
                    Dim newUser As New User With {.FirstName = thisRequest.FirstName, .LastName = thisRequest.LastName, .Email = thisRequest.Email, .DateCreated = Now, .UserTypeID = 1} 'usertype: user
                    newUser.PasswordHash = HashTools.CreateHash(thisRequest.Password)
                    If Not thisRequest.EmployerCode = Nothing Then
                        Dim existingCompany As Company = (From c In db.Companys Where c.EmployerCode = thisRequest.EmployerCode).FirstOrDefault
                        If Not existingCompany Is Nothing Then
                            newUser.CompanyID = existingCompany.CompanyID
                        Else
                            sbStatusMessage.Append("Employer Code was invalid.")
                            LogError("A user registered with a bad employee code: " + thisRequest.EmployerCode + " Email: " + thisRequest.Email, intAppID, Now)
                        End If
                    End If
                    db.Users.Add(newUser)
                    db.SaveChanges()
                    userResponse = New dcUserResponse(newUser)
                End If

            End Using
            userResponse.Status.Notes = sbStatusMessage.ToString
            status = Net.HttpStatusCode.Created
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "Registration Error", intAppID, Now)
            Return New dcUserResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during Registration.", .ExMessage = ex.ToString, .Successful = False, .Notes = "request: " + thisRequest.toString}}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

        Return userResponse
    End Function

    Public Function InsertUserPrefs(ByVal thisRequest As dcUserPrefsRequest) As dcOperationStatus Implements IService1.InsertUserPrefs
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK

        Try
            Using db As New SweatWorksEntities
                'amenitys
                Dim existingUserPrefAmenitys As List(Of UserPrefAmenity) = (From upa In db.UserPrefAmenitys Where upa.UserID = thisRequest.UserID).ToList
                If Not existingUserPrefAmenitys Is Nothing Then
                    db.UserPrefAmenitys.RemoveRange(existingUserPrefAmenitys)
                    db.SaveChanges()
                End If

                For Each thisAmenity As dcUserPrefAmenityRequest In thisRequest.Amenitys
                    Dim newUserPrefAmenity As New UserPrefAmenity With {.UserID = thisRequest.UserID, .Rank = thisAmenity.Rank, .AmenityID = thisAmenity.AmenityID}
                    db.UserPrefAmenitys.Add(newUserPrefAmenity)
                Next

                'gym class categories
                Dim existingCategories As List(Of UserPrefClassCategory) = (From upcc In db.UserPrefClassCategorys Where upcc.UserID = thisRequest.UserID).ToList
                If Not existingCategories Is Nothing Then
                    db.UserPrefClassCategorys.RemoveRange(existingCategories)
                    db.SaveChanges()
                End If
                For Each intCategoryID As Integer In thisRequest.GymClassCategoryIDs
                    Dim newUserPrefCategory As New UserPrefClassCategory With {.UserID = thisRequest.UserID, .ClassCategoryID = intCategoryID}
                    db.UserPrefClassCategorys.Add(newUserPrefCategory)
                Next

                db.SaveChanges()
                Return New dcOperationStatus With {.Successful = True}
            End Using
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "InsertUserPrefs Error", intAppID, Now)
            Return New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during SearchGyms.", .ExMessage = ex.ToString, .Successful = False}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

    End Function

    Public Function ResetPassword(ByVal thisRequest As dcPasswordResetRequest) As dcOperationStatus Implements IService1.ResetPassword
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK

        Try
            Using db As New SweatWorksEntities
                Dim existingUser As User = (From u In db.Users Where u.Email = thisRequest.Email).FirstOrDefault
                If existingUser Is Nothing Then
                    Return New dcOperationStatus With {.ErrorMessageForUser = "This email is not affililated with an existing account.", .Successful = False}
                Else
                    Dim strNewPassword As String = GenerateTempPassword()
                    existingUser.PasswordHash = HashTools.CreateHash(strNewPassword)
                    existingUser.IsTempPassword = True
                    db.SaveChanges()
                    Notify.SendEmail("smtp.sendgrid.net", "azure_321a635462b90efaff746c3214d6ba1e@azure.com", "Synapse1", "support@sweatquest.com", existingUser.Email,
                                     "Account support from SweatWorks", BuildNewPasswordMessage(strNewPassword))
                End If

                db.SaveChanges()
                Return New dcOperationStatus With {.Successful = True}
            End Using
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "ResetPassword Error", intAppID, Now)
            Return New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during ResetPassword.", .ExMessage = ex.ToString, .Successful = False}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

    End Function

    Public Function ChangePassword(ByVal thisRequest As dcPasswordChangeRequest) As dcOperationStatus Implements IService1.ChangePassword
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK

        Try
            Using db As New SweatWorksEntities
                Dim existingUser As User = (From u In db.Users Where u.UserID = thisRequest.UserID).FirstOrDefault
                If existingUser Is Nothing Then
                    Return New dcOperationStatus With {.ErrorMessageForUser = "This UserID is not affililated with an existing account.", .Successful = False}
                Else
                    If HashTools.ValidatePassword(thisRequest.OldPassword, existingUser.PasswordHash) Then
                        existingUser.PasswordHash = HashTools.CreateHash(thisRequest.NewPassword)
                        existingUser.IsTempPassword = False
                        db.SaveChanges()
                        Return New dcOperationStatus With {.Successful = True, .Notes = "Password Changed."}
                    Else
                        status = Net.HttpStatusCode.BadRequest
                        LogError("ChangePassword Error: OldPassword was incorrect", intAppID, Now)
                        Return New dcOperationStatus With {.ErrorMessageForUser = "Old password incorrect.", .Successful = False}
                    End If

                End If
            End Using
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "ChangePassword Error", intAppID, Now)
            Return New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during ChangePassword.", .ExMessage = ex.ToString, .Successful = False}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

    End Function

    Private Function GenerateTempPassword() As String
        Dim strCode = Nothing

        Dim alphaLetters() As String = {"A", "a", "B", "b", "1", "C", "c", "2", "D", "d", "3", "E", "e", "4", "F", "f", "5", "G", "g", "6", "H", "h", "7", "i", "8", "J", "j", "9", "K", "k", "L", "M", "m", "N", "n", "P", "p", "V", "v", "W", "w"}
        Dim alphaCount As Integer = alphaLetters.Length

        For i As Integer = 0 To 6
            Dim randNumber = GetRandomNumber(0, alphaCount - 1)
            strCode += alphaLetters(randNumber)
        Next

        Return "tmp-" + strCode
    End Function

    Dim objRandom As New System.Random(CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))
    Public Function GetRandomNumber(Optional ByVal Low As Integer = 1, Optional ByVal High As Integer = 100) As Integer
        ' Returns a random number,
        ' between the optional Low and High parameters
        Return objRandom.Next(Low, High + 1)
    End Function

    Public Shared Function BuildNewPasswordMessage(ByVal strNewPassword As String) As String
        Dim sbMessage As New StringBuilder
        sbMessage.Append("A password reset was requested through the SweatWorks system.  If you did not create this request, email support at <a href=""emailto:support@SweatQuest.com"">support@sweatquest.com</a><br><br>")
        sbMessage.Append("Your temporary password is <b>" + strNewPassword + "</b>")
        Return sbMessage.ToString
    End Function

#End Region

#Region "Gyms"

    Public Function GetGymDetails(ByVal strGymID As String) As dcGymDetailsResponse Implements IService1.GetGymDetails
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcGymDetailsResponse

        If Not IsNumeric(strGymID) Then
            LogError("Error in GetGymDetails.  Invalid gymID: " + strGymID, intAppID, Now)
            status = Net.HttpStatusCode.BadRequest
            Return Nothing
        End If

        Dim intGymID As Integer = CInt(strGymID)

        Try
            Using db As New SweatWorksEntities
                Dim thisGym As Gym = (From g In db.Gyms Where g.GymID = intGymID).FirstOrDefault
                Return New dcGymDetailsResponse(thisGym)
            End Using
        Catch ex As Exception
            LogError(ex, "Error in GetGymDetails.  strGymID: " + strGymID, intAppID, Now)
            status = Net.HttpStatusCode.Conflict
            Return thisResponse
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try
    End Function

    Public Function SearchGyms(ByVal thisRequest As dcGymSearchRequest) As dcGymsResponse Implements IService1.SearchGyms
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim gymsResponse As New dcGymsResponse

        Try
            Using db As New SweatWorksEntities
                Dim gyms As List(Of Gym) = Nothing

                '1. look for location first
                If Not thisRequest.Latitude = Nothing And Not thisRequest.Longitude = Nothing And Not thisRequest.MaxDistanceInMiles = Nothing Then
                    'lat/long distance
                    Dim dLat As Double = CDbl(thisRequest.Latitude)
                    Dim dLong As Double = CDbl(thisRequest.Longitude)
                    Dim nearbyGymsResult As ObjectResult(Of GetNearbyGyms_Result) = db.GetNearbyGyms(dLat, dLong, thisRequest.MaxDistanceInMiles)
                    If Not nearbyGymsResult Is Nothing Then
                        gyms = New List(Of Gym)
                        For Each thisNearResult As GetNearbyGyms_Result In nearbyGymsResult
                            Dim thisGym As Gym = (From g In db.Gyms Where g.GymID = thisNearResult.GymID).FirstOrDefault
                            If Not thisGym Is Nothing Then
                                gyms.Add(thisGym)
                            End If
                        Next
                    End If

                ElseIf Not thisRequest.City = Nothing And Not thisRequest.State = Nothing Then
                    'city/state
                    gyms = (From g In db.Gyms Where g.City = thisRequest.City And g.State = thisRequest.State And g.DateDeleted Is Nothing And g.GymAmenitys.Count > 0).ToList
                ElseIf Not thisRequest.Zip = Nothing Then
                    'zip
                    gyms = (From g In db.Gyms Where g.Zip = thisRequest.Zip And g.DateDeleted Is Nothing And g.GymAmenitys.Count > 0).ToList
                End If

                '2. narrow by name, unless location results are empty, then start new search
                If Not thisRequest.Name = Nothing Then
                    If gyms Is Nothing Then
                        gyms = (From g In db.Gyms Where g.Name.ToLower.Contains(thisRequest.Name.ToLower) And g.DateDeleted Is Nothing And g.GymAmenitys.Count > 0).ToList
                    Else
                        gyms = (From g In gyms Where g.Name.ToLower.Contains(thisRequest.Name.ToLower) And g.DateDeleted Is Nothing).ToList
                    End If
                End If

                'sort gyms by preferences if they exist?
                If Not thisRequest.UserID = Nothing Then
                    gyms = (From g In gyms Where g.DateDeleted Is Nothing
                            Order By If((From ga In g.GymAmenitys
                                         Join upa In db.UserPrefAmenitys On ga.AmenityID Equals upa.AmenityID
                                         Where upa.UserID = thisRequest.UserID
                                         Order By upa.Rank
                                         Select CType(upa.Rank, Integer?)).FirstOrDefault, 11),
                        If((From ga In g.GymClasses
                            Join upc In db.UserPrefClassCategorys On ga.GymClassCategoryID Equals upc.GymClassCategory.GymClassCategoryID
                            Where upc.UserID = thisRequest.UserID
                            Select CType(upc.ClassCategoryID, Integer?)).FirstOrDefault, 0) Descending).Take(50).ToList


                End If
                If Not gyms Is Nothing Then
                    For Each thisGym As Gym In gyms
                        gymsResponse.Gyms.Add(New dcGymResponse(thisGym))
                    Next
                    gymsResponse.Status = New dcOperationStatus With {.Successful = True}
                Else
                    gymsResponse.Status = New dcOperationStatus With {.Successful = True, .ErrorMessageForUser = "No gyms found."}
                End If
            End Using

        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "Registration Error", intAppID, Now)
            Return New dcGymsResponse With {.Status = New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during SearchGyms.", .ExMessage = ex.ToString, .Successful = False}}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

        Return gymsResponse
    End Function

    Public Function GetAllCityStates() As List(Of String) Implements IService1.GetAllCityStates
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcGymDetailsResponse

        Try
            Using db As New SweatWorksEntities
                Return (From g In db.Gyms Select citystate = g.City + ", " + g.State Distinct Order By citystate).ToList
            End Using
        Catch ex As Exception
            LogError(ex, "Error in GetAllZips.", intAppID, Now)
            status = Net.HttpStatusCode.Conflict
            Return Nothing
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try
    End Function

    Public Function GetAllZips() As List(Of String) Implements IService1.GetAllZips
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcGymDetailsResponse

        Try
            Using db As New SweatWorksEntities
                Return (From g In db.Gyms Select g.Zip Distinct).ToList
            End Using
        Catch ex As Exception
            LogError(ex, "Error in GetAllZips.", intAppID, Now)
            status = Net.HttpStatusCode.Conflict
            Return Nothing
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try
    End Function

#End Region

#Region "Fit Bank - Checkins, Dashboard, etc."

    Public Function InsertCheckin(ByVal thisRequest As dcCheckinInsertRequest) As dcOperationStatus Implements IService1.InsertCheckin
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK

        Try
            Using db As New SweatWorksEntities
                Dim newCheckin As New Checkin With {.UserID = thisRequest.UserID, .GymID = thisRequest.GymID}
                newCheckin.DateCreated = CType(thisRequest.DateCreated, DateTimeOffset).ToString("M-d-yyyy h:mm tt zzz")
                db.Checkins.Add(newCheckin)
                db.SaveChanges()
                Return New dcOperationStatus With {.Successful = True}
            End Using
        Catch ex As Exception
            status = Net.HttpStatusCode.Conflict
            LogError(ex, "InsertUserPrefs Error", intAppID, Now)
            Return New dcOperationStatus With {.ErrorMessageForUser = "Exception occured during InsertCheckin.", .ExMessage = ex.ToString, .Successful = False}
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

    End Function

    Public Function GetFitBankDashboard(ByVal strUserID As String) As dcFitBankDashboardResponse Implements IService1.GetFitBankDashboard
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcFitBankDashboardResponse

        If Not IsNumeric(strUserID) Then
            LogError("Error in GetFitBankDashboard.  Invalid user id: " + strUserID, intAppID, Now)
            status = Net.HttpStatusCode.BadRequest
            thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetFitBankDashboard.  Invalid user id: " + strUserID}
            Return thisResponse
        End If

        Dim intUserID As Integer = CInt(strUserID)

        Try
            Using db As New SweatWorksEntities
                Dim thisUser As User = (From u In db.Users Where u.UserID = intUserID).FirstOrDefault
                If Not thisUser Is Nothing Then
                    'favorite gyms
                    Dim results = (From checkins In thisUser.Checkins Group checkins.Gym By checkins.Gym Into favgyms = Group Order By favgyms.Count Descending Select New With {favgyms.Count, Gym.Name, Gym.GymID}).Take(3).ToList
                    For Each thisResult In results
                        thisResponse.Favorites.Add(New dcGymListing With {.Name = thisResult.Name, .GymID = thisResult.GymID})
                    Next

                    'recent checkins
                    Dim recentCheckins As List(Of Checkin) = (From rc In thisUser.Checkins Order By rc.DateCreated Descending).Take(1).ToList
                    For Each thisCheckin In recentCheckins
                        Dim thisDCcheckin As dcCheckin = New dcCheckin(thisCheckin)
                        thisDCcheckin.GymName = thisCheckin.Gym.Name
                        thisResponse.RecentCheckins.Add(thisDCcheckin)
                    Next

                    'unredeemed checkins
                    thisResponse.TotalUnredeemedCheckIns = (From c In thisUser.Checkins Where c.RedeemedReward Is Nothing).Count
                    'redeemed checkins
                    thisResponse.TotalRedeemedCheckIns = (From c In thisUser.Checkins Where Not c.RedeemedReward Is Nothing).Count
                    'total checkins needed for next reward
                    Dim defaultReward As CompanyReward = thisUser.Company.CompanyRewards.FirstOrDefault()
                    If Not defaultReward Is Nothing Then
                        thisResponse.CheckinsUntilNextReward = defaultReward.CheckinsNeeded - thisResponse.TotalUnredeemedCheckIns
                        thisResponse.RewardCost = defaultReward.CheckinsNeeded
                    End If
                    'TODO: deposits?
                    thisResponse.TotalRedeemedRewards = (From rr In thisUser.RedeemedRewards).Count
                Else
                    LogError("Error in GetFitBankDashboard.  User not found.  strUserID: " + strUserID, intAppID, Now)
                    thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetFitBankDashboard.  User not found.  strUserID: " + strUserID}
                    status = Net.HttpStatusCode.Conflict
                End If
            End Using
        Catch ex As Exception
            LogError(ex, "Error in GetFitBankDashboard.  strUserID: " + strUserID, intAppID, Now)
            thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetFitBankDashboard.  strUserID: " + strUserID, .ExMessage = ex.Message}
            status = Net.HttpStatusCode.Conflict
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

        Return thisResponse
    End Function

    Public Function GetCheckins(ByVal strUserID As String) As dcCheckinsResponse Implements IService1.GetCheckins
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcCheckinsResponse

        If Not IsNumeric(strUserID) Then
            LogError("Error in GetCheckins.  Invalid user id: " + strUserID, intAppID, Now)
            status = Net.HttpStatusCode.BadRequest
            thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetCheckins.  Invalid user id: " + strUserID}
            Return thisResponse
        End If

        Dim intUserID As Integer = CInt(strUserID)

        Try
            Using db As New SweatWorksEntities
                Dim thisUser As User = (From u In db.Users Where u.UserID = intUserID).FirstOrDefault
                If Not thisUser Is Nothing Then
                    Dim checkins As List(Of Checkin) = (From c In thisUser.Checkins Order By c.DateCreated Descending).ToList
                    For Each thisCheckin As Checkin In checkins
                        Dim thisDCcheckin As dcCheckin = New dcCheckin(thisCheckin)
                        thisDCcheckin.GymName = thisCheckin.Gym.Name
                        thisResponse.Checkins.Add(thisDCcheckin)
                    Next
                    thisResponse.Status = New dcOperationStatus With {.Successful = True}
                Else
                    LogError("Error in GetFitBankDashboard.  User not found.  strUserID: " + strUserID, intAppID, Now)
                    thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetCheckins.  User not found.  strUserID: " + strUserID}
                    status = Net.HttpStatusCode.Conflict
                End If
            End Using
        Catch ex As Exception
            LogError(ex, "Error in GetFitBankDashboard.  strUserID: " + strUserID, intAppID, Now)
            thisResponse.Status = New dcOperationStatus With {.Successful = False, .Notes = "Error in GetCheckins.  strUserID: " + strUserID, .ExMessage = ex.Message}
            status = Net.HttpStatusCode.Conflict
        Finally
            ctx.OutgoingResponse.StatusCode = status
        End Try

        Return thisResponse
    End Function

#End Region


End Class
