Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymWebLinkResponse")> _
    Public Class dcGymWebLinkResponse
        Private _GymWebLinkID As Integer
        Private _WebLinkTypeName As String
        Private _URL As String
        Private _WebLinkTypeID As Integer
        Private _WebLinkTypeIconURL As String

        <DataMember(IsRequired:=True)>
        Public Property GymWebLinkID() As Integer
            Get
                Return _GymWebLinkID
            End Get
            Set(ByVal value As Integer)
                _GymWebLinkID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property WebLinkTypeID() As Integer
            Get
                Return _WebLinkTypeID
            End Get
            Set(ByVal value As Integer)
                _WebLinkTypeID = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property WebLinkTypeName() As String
            Get
                Return _WebLinkTypeName
            End Get
            Set(ByVal value As String)
                _WebLinkTypeName = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property WebLinkTypeIconURL() As String
            Get
                Return _WebLinkTypeIconURL
            End Get
            Set(ByVal value As String)
                _WebLinkTypeIconURL = value
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

        Public Sub New(ByVal thisGymWebLink As GymWebLink)
            _GymWebLinkID = thisGymWebLink.GymWebLinkID
            _WebLinkTypeName = thisGymWebLink.WebLinkType.Description
            _URL = thisGymWebLink.URL
            _WebLinkTypeID = thisGymWebLink.WebLinkTypeID
            _WebLinkTypeIconURL = thisGymWebLink.WebLinkType.IconURL
          
        End Sub

        
    End Class



End Namespace