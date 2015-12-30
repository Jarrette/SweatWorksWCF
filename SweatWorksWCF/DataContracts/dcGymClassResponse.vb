Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymClassResponse")> _
    Public Class dcGymClassResponse
        Private _GymClassID As Integer
        Private _Title As String
        Private _Description As String

        <DataMember(IsRequired:=True)>
        Public Property GymClassID() As Integer
            Get
                Return _GymClassID
            End Get
            Set(ByVal value As Integer)
                _GymClassID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal value As String)
                _Title = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property


        Public Sub New()
        End Sub

        Public Sub New(ByVal thisGymClass As GymClass)
            _GymClassID = thisGymClass.GymClassID
            _Title = thisGymClass.Title
            _Description = thisGymClass.Description
        End Sub

    End Class



End Namespace