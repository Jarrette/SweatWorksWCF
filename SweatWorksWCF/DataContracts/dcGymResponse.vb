Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymResponse")> _
    Public Class dcGymResponse
        Private _GymID As Integer
        Private _Name As String
        Private _Description As String
        Private _Address As String
        Private _City As String
        Private _State As String
        Private _Zip As String
        Private _Latitude As String
        Private _Longitude As String
        Private _GymImageURL As String

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

        <DataMember(IsRequired:=True)>
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Address() As String
            Get
                Return _Address
            End Get
            Set(ByVal value As String)
                _Address = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property City() As String
            Get
                Return _City
            End Get
            Set(ByVal value As String)
                _City = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property State() As String
            Get
                Return _State
            End Get
            Set(ByVal value As String)
                _State = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Zip() As String
            Get
                Return _Zip
            End Get
            Set(ByVal value As String)
                _Zip = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Latitude() As String
            Get
                Return _Latitude
            End Get
            Set(ByVal value As String)
                _Latitude = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Longitude() As String
            Get
                Return _Longitude
            End Get
            Set(ByVal value As String)
                _Longitude = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property GymImageURL() As String
            Get
                Return _GymImageURL
            End Get
            Set(ByVal value As String)
                _GymImageURL = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal thisGym As Gym)
            _GymID = thisGym.GymID
            _Name = thisGym.Name
            _Description = thisGym.Description
            _Address = thisGym.Address1
            _City = thisGym.City
            _State = thisGym.State
            _Zip = thisGym.Zip
            _Latitude = thisGym.LatCentroid
            _Longitude = thisGym.LongCentroid
            If Not thisGym.GymPhotos.FirstOrDefault Is Nothing Then
                _GymImageURL = thisGym.GymPhotos.FirstOrDefault.URL
            End If
        End Sub

        Public Sub New(ByVal thisGym As GetNearbyGyms_Result)
            _GymID = thisGym.GymID
            _Name = thisGym.Name
            _Description = thisGym.Description
            _Address = thisGym.Address1
            _City = thisGym.City
            _State = thisGym.State
            _Zip = thisGym.Zip
            _Latitude = thisGym.LatCentroid
            _Longitude = thisGym.LongCentroid
            If Not thisGym.URL Is Nothing Then
                _GymImageURL = thisGym.URL
            End If
        End Sub
    End Class



End Namespace