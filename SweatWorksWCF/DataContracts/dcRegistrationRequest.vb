Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcRegistrationRequest")> _
    Public Class dcRegistrationRequest
        Private _FirstName As String
        Private _LastName As String
        Private _Email As String
        Private _Password As String
        Private _EmployerCode As String

        <DataMember(IsRequired:=True)>
        Public Property FirstName() As String
            Get
                Return _FirstName
            End Get
            Set(ByVal value As String)
                _FirstName = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property LastName() As String
            Get
                Return _LastName
            End Get
            Set(ByVal value As String)
                _LastName = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Password() As String
            Get
                Return _Password
            End Get
            Set(ByVal value As String)
                _Password = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property EmployerCode() As String
            Get
                Return _EmployerCode
            End Get
            Set(ByVal value As String)
                _EmployerCode = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Overrides Function toString() As String
            Return "email: " + Email + " First Name: " + FirstName + " Last Name: " + LastName + " password: " + Password
        End Function

    End Class

End Namespace