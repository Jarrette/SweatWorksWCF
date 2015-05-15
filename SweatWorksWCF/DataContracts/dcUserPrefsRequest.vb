Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcUserPrefsRequest")> _
    Public Class dcUserPrefsRequest
        Private _UserID As Integer
        Private _Amenitys As List(Of dcUserPrefAmenityRequest)
        Private _GymClassCategoryIDs As List(Of Integer)
      

        <DataMember(IsRequired:=True)>
        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property Amenitys() As List(Of dcUserPrefAmenityRequest)
            Get
                Return _Amenitys
            End Get
            Set(ByVal value As List(Of dcUserPrefAmenityRequest))
                _Amenitys = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property GymClassCategoryIDs() As List(Of Integer)
            Get
                Return _GymClassCategoryIDs
            End Get
            Set(ByVal value As List(Of Integer))
                _GymClassCategoryIDs = value
            End Set
        End Property

        Public Sub New()
        End Sub



    End Class

End Namespace