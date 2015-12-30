Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcCheckin")> _
    Public Class dcCheckin
        Private _UserID As Integer
        Private _GymID As Integer
        Private _GymName As String
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
        Public Property GymName() As String
            Get
                Return _GymName
            End Get
            Set(ByVal value As String)
                _GymName = value
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

        Public Sub New(ByVal thisCheckin As Checkin)
            _GymID = thisCheckin.GymID

            _DateCreated = thisCheckin.DateCreated.ToString("M/d/yyyy h:mm tt")
            _UserID = thisCheckin.UserID
        End Sub

    End Class

End Namespace