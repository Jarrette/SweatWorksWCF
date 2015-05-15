Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymPhotoResponse")> _
    Public Class dcGymPhotoResponse
        Private _GymPhotoID As Integer
        Private _URL As String

        <DataMember(IsRequired:=True)>
        Public Property GymPhotoID() As Integer
            Get
                Return _GymPhotoID
            End Get
            Set(ByVal value As Integer)
                _GymPhotoID = value
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


        Public Sub New()
        End Sub

        Public Sub New(ByVal thisGymPhoto As GymPhoto)
            _GymPhotoID = thisGymPhoto.GymPhotoID
            _URL = thisGymPhoto.URL
        End Sub


    End Class



End Namespace