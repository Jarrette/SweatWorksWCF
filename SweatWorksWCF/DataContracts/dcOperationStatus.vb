Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
        <DataContract()>
    Public Class dcOperationStatus
        Private _Successful As Boolean = True
        Private _ExMessage As String
        Private _Notes As String
        Private _IdentityID As Integer
        Private _ErrorMessageForUser As String

        <DataMember()>
        Public Property Successful() As Boolean
            Get
                Return _Successful
            End Get
            Set(ByVal value As Boolean)
                _Successful = value
            End Set
        End Property

        <DataMember()>
        Public Property ExMessage() As String
            Get
                Return _ExMessage
            End Get
            Set(ByVal value As String)
                _ExMessage = value
            End Set
        End Property

        <DataMember()>
        Public Property Notes() As String
            Get
                Return _Notes
            End Get
            Set(ByVal value As String)
                _Notes = value
            End Set
        End Property

        <DataMember()>
        Public Property IdentityID() As Integer
            Get
                Return _IdentityID
            End Get
            Set(ByVal value As Integer)
                _IdentityID = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorMessageForUser() As String
            Get
                Return _ErrorMessageForUser
            End Get
            Set(ByVal value As String)
                _ErrorMessageForUser = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal ex As Exception, ByVal strErrorMessage As String)
            _Successful = False

            Dim sb As New StringBuilder
            If Not ex.Message Is Nothing Then
                sb.Append(ex.Message)
            End If
            If Not ex.InnerException Is Nothing Then
                sb.Append(ex.InnerException.Message)
            End If
            _ErrorMessageForUser = strErrorMessage
        End Sub

    End Class

End Namespace