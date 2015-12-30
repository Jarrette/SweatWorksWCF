Imports SweatWorksWCF.DataContracts

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IService1" in both code and config file together.
<ServiceContract()>
Public Interface IService1


    'SITE USER and AUTHENTICATION----------------------------------------------------------------------------------

    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="login", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function Login(ByVal thisRequest As dcLoginRequest) As dcUserResponse

    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="register", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function Register(ByVal thisRequest As dcRegistrationRequest) As dcUserResponse

    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="prefs/insert", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function InsertUserPrefs(ByVal thisRequest As dcUserPrefsRequest) As dcOperationStatus

    <OperationContract()>
    <WebInvoke(Method:="POST", UriTemplate:="password/reset", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)>
    Function ResetPassword(ByVal thisRequest As dcPasswordResetRequest) As dcOperationStatus

    <OperationContract()>
    <WebInvoke(Method:="POST", UriTemplate:="prefs/insert", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)>
    Function ChangePassword(ByVal thisRequest As dcPasswordChangeRequest) As dcOperationStatus


    'GYMS----------------------------------------------------------------------------------
    <OperationContract()> _
    <WebGet(UriTemplate:="gym/details/{strGymID}", responseformat:=WebMessageFormat.Json)> _
    Function GetGymDetails(ByVal strGymID As String) As dcGymDetailsResponse

    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="gyms/search", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function SearchGyms(ByVal thisRequest As dcGymSearchRequest) As dcGymsResponse

    <OperationContract()> _
    <WebGet(UriTemplate:="citystates", responseformat:=WebMessageFormat.Json)> _
    Function GetAllCityStates() As List(Of String)

    <OperationContract()> _
    <WebGet(UriTemplate:="zips", responseformat:=WebMessageFormat.Json)> _
    Function GetAllZips() As List(Of String)


    'CHECKINS----------------------------------------------------------------------------------
    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="checkin/insert", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function InsertCheckin(ByVal thisRequest As dcCheckinInsertRequest) As dcOperationStatus

    <OperationContract()> _
    <WebGet(UriTemplate:="fitbank/dashboard/{strUserID}", responseformat:=WebMessageFormat.Json)> _
    Function GetFitBankDashboard(ByVal strUserID As String) As dcFitBankDashboardResponse

    <OperationContract()> _
    <WebGet(UriTemplate:="checkins/{strUserID}", responseformat:=WebMessageFormat.Json)> _
    Function GetCheckins(ByVal strUserID As String) As dcCheckinsResponse

End Interface


