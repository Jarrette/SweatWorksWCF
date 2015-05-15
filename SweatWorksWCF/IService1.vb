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


    'GYMS----------------------------------------------------------------------------------
    <OperationContract()> _
    <WebGet(UriTemplate:="gym/details/{strGymID}", responseformat:=WebMessageFormat.Json)> _
    Function GetGymDetails(ByVal strGymID As String) As dcGymDetailsResponse

    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="gyms/search", RequestFormat:=WebMessageFormat.Json, responseformat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare)> _
    Function SearchGyms(ByVal thisRequest As dcGymSearchRequest) As dcGymsResponse



End Interface


