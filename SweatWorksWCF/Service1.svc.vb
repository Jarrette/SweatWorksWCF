Imports SweatWorksWCF.DataContracts
Imports SweatWorksData
Imports SweatWorksData.ErrorLogging
Imports System.Data.Entity.Core.Objects

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
            Using db As New SweatWorksEntities
                Dim thisUser As User = (From u In db.Users Where u.Email = strEmail).FirstOrDefault

                If Not thisUser Is Nothing Then

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
                    db.Users.Add(newUser)
                    db.SaveChanges()
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

#End Region

#Region "Gyms"

    Public Function GetGymDetails(ByVal strGymID As String) As dcGymDetailsResponse Implements IService1.GetGymDetails
        Dim ctx As WebOperationContext = WebOperationContext.Current
        Dim status As System.Net.HttpStatusCode = System.Net.HttpStatusCode.OK
        Dim thisResponse As New dcGymDetailsResponse

        If Not IsNumeric(strGymID) Then
            LogError("Error in GetGymDetails.  Invalid parameter: " + strGymID, intAppID, Now)
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
                    For Each thisNearResult As GetNearbyGyms_Result In nearbyGymsResult
                        Dim thisGym As Gym = (From g In db.Gyms Where g.GymID = thisNearResult.GymID).FirstOrDefault
                        If Not thisGym Is Nothing Then
                            gyms.Add(thisGym)
                        End If
                    Next
                ElseIf Not thisRequest.City = Nothing And Not thisRequest.State = Nothing Then
                    'city/state
                    gyms = (From g In db.Gyms Where g.City = thisRequest.City And g.State = thisRequest.State).ToList
                ElseIf Not thisRequest.Zip = Nothing Then
                    'zip
                    gyms = (From g In db.Gyms Where g.Zip = thisRequest.Zip).ToList
                End If

                '2. narrow by name, unless location results are empty, then start new search
                If Not thisRequest.Name = Nothing Then
                    If gyms Is Nothing Then
                        gyms = (From g In db.Gyms Where g.Name.Contains(thisRequest.Name)).ToList
                    Else
                        gyms = (From g In gyms Where g.Name.Contains(thisRequest.Name)).ToList
                    End If
                End If

                'sort gyms by preferences if they exist?
                If Not thisRequest.UserID = Nothing Then
                    gyms = (From g In gyms _
                        Order By If((From ga In g.GymAmenitys _
                        Join upa In db.UserPrefAmenitys On ga.AmenityID Equals upa.AmenityID _
                        Where upa.UserID = thisRequest.UserID _
                        Order By upa.Rank _
                        Select CType(upa.Rank, Integer?)).FirstOrDefault, 11), _
                        If((From ga In g.GymClasses _
                        Join upc In db.UserPrefClassCategorys On ga.GymClassCategoryID Equals upc.GymClassCategory.GymClassCategoryID _
                        Where upc.UserID = thisRequest.UserID _
                        Select CType(upc.ClassCategoryID, Integer?)).FirstOrDefault, 0) Descending).ToList

                    
                End If

                For Each thisGym As Gym In gyms
                    gymsResponse.Gyms.Add(New dcGymResponse(thisGym))
                Next
                gymsResponse.Status = New dcOperationStatus With {.Successful = True}
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

#End Region




End Class
