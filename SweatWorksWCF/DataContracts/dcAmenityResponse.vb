Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymAmenityResponse")> _
    Public Class dcGymAmenityResponse
        Private _GymAmenityID As Integer
        Private _AmenityID As Integer
        Private _Description As String

        <DataMember(IsRequired:=True)>
        Public Property GymAmenityID() As Integer
            Get
                Return _GymAmenityID
            End Get
            Set(ByVal value As Integer)
                _GymAmenityID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property AmenityID() As Integer
            Get
                Return _AmenityID
            End Get
            Set(ByVal value As Integer)
                _AmenityID = value
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

        Public Sub New(ByVal thisAmenity As GymAmenity)
            _GymAmenityID = thisAmenity.GymAmenityID
            _AmenityID = thisAmenity.AmenityID
            _Description = thisAmenity.Amenity.Description
        End Sub

    End Class



End Namespace