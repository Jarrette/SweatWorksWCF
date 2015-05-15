Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcUserPrefAmenityRequest")> _
    Public Class dcUserPrefAmenityRequest
        Private _AmenityID As Integer
        Private _Rank As Integer


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
        Public Property Rank() As Integer
            Get
                Return _Rank
            End Get
            Set(ByVal value As Integer)
                _Rank = value
            End Set
        End Property


        Public Sub New()
        End Sub



    End Class

End Namespace