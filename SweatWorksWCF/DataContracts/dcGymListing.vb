Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymListing")> _
    Public Class dcGymListing
        Private _GymID As Integer
        Private _Name As String


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
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property


        Public Sub New()
        End Sub

        Public Sub New(ByVal thisGym As Gym)
            _GymID = thisGym.GymID
            _Name = thisGym.Name
        
        End Sub

       
    End Class



End Namespace