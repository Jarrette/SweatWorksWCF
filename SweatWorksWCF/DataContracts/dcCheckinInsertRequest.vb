Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcCheckinInsertRequest")> _
    Public Class dcCheckinInsertRequest
        Private _UserID As Integer
        Private _GymID As Integer
        Private _DateCreated As String

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
        Public Property GymID() As Integer
            Get
                Return _GymID
            End Get
            Set(ByVal value As Integer)
                _GymID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property DateCreated() As String
            Get
                Return _DateCreated
            End Get
            Set(ByVal value As String)
                _DateCreated = value
            End Set
        End Property

        Public Sub New()
        End Sub

    End Class

End Namespace