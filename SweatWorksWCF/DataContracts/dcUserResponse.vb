Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcUserResponse")> _
    Public Class dcUserResponse
        Private _UserID As Integer
        Private _UserTypeID As Integer
        Private _Email As String
        Private _FirstName As String
        Private _LastName As String
        Private _UserImageURL As String
        Private _Status As dcOperationStatus
        Private _CompanyID As Integer
        Private _CompanyName As String
        Private _IsTempPassword As Boolean

        <DataMember(IsRequired:=True)>
        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property UserTypeID() As Integer
            Get
                Return _UserTypeID
            End Get
            Set(ByVal value As Integer)
                _UserTypeID = value
            End Set
        End Property

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
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
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

        <DataMember(IsRequired:=False)>
        Public Property UserImageURL() As String
            Get
                Return _UserImageURL
            End Get
            Set(ByVal value As String)
                _UserImageURL = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Status() As dcOperationStatus
            Get
                Return _Status
            End Get
            Set(ByVal value As dcOperationStatus)
                _Status = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property CompanyID() As Integer
            Get
                Return _CompanyID
            End Get
            Set(ByVal value As Integer)
                _CompanyID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property CompanyName() As String
            Get
                Return _CompanyName
            End Get
            Set(ByVal value As String)
                _CompanyName = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property IsTempPassword() As Boolean
            Get
                Return _IsTempPassword
            End Get
            Set(ByVal value As Boolean)
                _IsTempPassword = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal thisUser As User)
            _UserID = thisUser.UserID
            _Email = thisUser.Email
            _UserTypeID = thisUser.UserTypeID
            _FirstName = thisUser.FirstName
            _LastName = thisUser.LastName
            _UserImageURL = thisUser.UserImageURL
            _Status = New dcOperationStatus With {.Successful = True, .IdentityID = thisUser.UserID.ToString}
            If Not thisUser.CompanyID Is Nothing Then
                _CompanyID = thisUser.CompanyID
            End If
            If Not thisUser.IsTempPassword Is Nothing Then
                _IsTempPassword = thisUser.IsTempPassword
            End If
        End Sub
    End Class



End Namespace