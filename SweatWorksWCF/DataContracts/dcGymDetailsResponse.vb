Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymDetailsResponse")> _
    Public Class dcGymDetailsResponse
        Inherits dcGymResponse
        Private _PhoneNumber As String
        Private _AreaCode As String
        Private _URL As String
        Private _GymAmenitys As New List(Of dcGymAmenityResponse)
        Private _GymPhotos As New List(Of dcGymPhotoResponse)
        Private _GymWebLinks As New List(Of dcGymWebLinkResponse)
        Private _GymClasses As New List(Of dcGymClassResponse)

        <DataMember(IsRequired:=True)>
        Public Property PhoneNumber() As String
            Get
                Return _PhoneNumber
            End Get
            Set(ByVal value As String)
                _PhoneNumber = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property AreaCode() As String
            Get
                Return _AreaCode
            End Get
            Set(ByVal value As String)
                _AreaCode = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property URL() As String
            Get
                Return _URL
            End Get
            Set(ByVal value As String)
                _URL = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property GymAmenitys() As List(Of dcGymAmenityResponse)
            Get
                Return _GymAmenitys
            End Get
            Set(ByVal value As List(Of dcGymAmenityResponse))
                _GymAmenitys = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property GymPhotos() As List(Of dcGymPhotoResponse)
            Get
                Return _GymPhotos
            End Get
            Set(ByVal value As List(Of dcGymPhotoResponse))
                _GymPhotos = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property GymWebLinks() As List(Of dcGymWebLinkResponse)
            Get
                Return _GymWebLinks
            End Get
            Set(ByVal value As List(Of dcGymWebLinkResponse))
                _GymWebLinks = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property GymClasses() As List(Of dcGymClassResponse)
            Get
                Return _GymClasses
            End Get
            Set(ByVal value As List(Of dcGymClassResponse))
                _GymClasses = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal thisGym As Gym)
            'standard
            GymID = thisGym.GymID
            Name = thisGym.Name
            Description = thisGym.Description
            Address = thisGym.Address1
            City = thisGym.City
            State = thisGym.State
            Zip = thisGym.Zip
            Latitude = thisGym.LatCentroid
            Longitude = thisGym.LongCentroid

            'detailed
            _PhoneNumber = thisGym.PhoneNumber
            _AreaCode = thisGym.AreaCode
            _URL = thisGym.URL
            For Each thisAmenity In thisGym.GymAmenitys
                _GymAmenitys.Add(New dcGymAmenityResponse(thisAmenity))
            Next
            For Each thisPhoto In thisGym.GymPhotos
                _GymPhotos.Add(New dcGymPhotoResponse(thisPhoto))
            Next
            For Each thisLink In thisGym.GymWebLinks
                _GymWebLinks.Add(New dcGymWebLinkResponse(thisLink))
            Next
            For Each thisClass In thisGym.GymClasses
                _GymClasses.Add(New dcGymClassResponse(thisClass))
            Next
        End Sub

    End Class



End Namespace