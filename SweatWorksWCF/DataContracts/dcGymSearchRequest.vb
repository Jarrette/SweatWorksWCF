Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymSearchRequest")> _
    Public Class dcGymSearchRequest
        Private _Name As String
        Private _Latitude As String
        Private _Longitude As String
        Private _MaxDistanceInMiles As Integer
        Private _City As String
        Private _State As String
        Private _Zip As String
        Private _UserID As Integer

        <DataMember(IsRequired:=False)>
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property Latitude() As String
            Get
                Return _Latitude
            End Get
            Set(ByVal value As String)
                _Latitude = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property Longitude() As String
            Get
                Return _Longitude
            End Get
            Set(ByVal value As String)
                _Longitude = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property MaxDistanceInMiles() As Integer
            Get
                Return _MaxDistanceInMiles
            End Get
            Set(ByVal value As Integer)
                _MaxDistanceInMiles = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property City() As String
            Get
                Return _City
            End Get
            Set(ByVal value As String)
                _City = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property State() As String
            Get
                Return _State
            End Get
            Set(ByVal value As String)
                _State = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property Zip() As String
            Get
                Return _Zip
            End Get
            Set(ByVal value As String)
                _Zip = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

        Public Sub New()
        End Sub



    End Class

End Namespace