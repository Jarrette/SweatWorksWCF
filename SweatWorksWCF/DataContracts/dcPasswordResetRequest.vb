Imports SweatWorksData

Namespace DataContracts

    <Serializable()>
    <DataContract(Name:="dcPasswordResetRequest")>
    Public Class dcPasswordResetRequest
        Private _Email As String

        <DataMember(IsRequired:=True)>
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        Public Sub New()
        End Sub

    End Class

End Namespace