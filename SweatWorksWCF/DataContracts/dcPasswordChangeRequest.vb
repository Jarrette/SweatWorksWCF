Imports SweatWorksData

Namespace DataContracts

    <Serializable()>
    <DataContract(Name:="dcPasswordChangeRequest")>
    Public Class dcPasswordChangeRequest
        Private _UserID As Integer
        Private _OldPassword As Integer
        Private _NewPassword As String

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
        Public Property OldPassword() As Integer
            Get
                Return _OldPassword
            End Get
            Set(ByVal value As Integer)
                _OldPassword = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property NewPassword() As String
            Get
                Return _NewPassword
            End Get
            Set(ByVal value As String)
                _NewPassword = value
            End Set
        End Property

        Public Sub New()
        End Sub

    End Class

End Namespace