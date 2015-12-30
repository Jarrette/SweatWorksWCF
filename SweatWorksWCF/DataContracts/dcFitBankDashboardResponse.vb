Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcFitBankDashboardResponse")> _
    Public Class dcFitBankDashboardResponse
        Private _TotalRedeemedCheckIns As Integer = 0
        Private _TotalUnredeemedCheckIns As Integer = 0
        Private _TotalRedeemedRewards As Integer = 0
        Private _CheckinsUntilNextReward As Integer
        Private _RewardCost As Integer
        Private _RecentCheckins As New List(Of dcCheckin)
        Private _Favorites As New List(Of dcGymListing)
        Private _Status As New dcOperationStatus

        <DataMember(IsRequired:=True)>
        Public Property TotalRedeemedCheckIns() As Integer
            Get
                Return _TotalRedeemedCheckIns
            End Get
            Set(ByVal value As Integer)
                _TotalRedeemedCheckIns = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property TotalUnredeemedCheckIns() As Integer
            Get
                Return _TotalUnredeemedCheckIns
            End Get
            Set(ByVal value As Integer)
                _TotalUnredeemedCheckIns = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property TotalRedeemedRewards() As Integer
            Get
                Return _TotalRedeemedRewards
            End Get
            Set(ByVal value As Integer)
                _TotalRedeemedRewards = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property CheckinsUntilNextReward() As Integer
            Get
                Return _CheckinsUntilNextReward
            End Get
            Set(ByVal value As Integer)
                _CheckinsUntilNextReward = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property RewardCost() As Integer
            Get
                Return _RewardCost
            End Get
            Set(ByVal value As Integer)
                _RewardCost = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property RecentCheckins() As List(Of dcCheckin)
            Get
                Return _RecentCheckins
            End Get
            Set(ByVal value As List(Of dcCheckin))
                _RecentCheckins = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Favorites() As List(Of dcGymListing)
            Get
                Return _Favorites
            End Get
            Set(ByVal value As List(Of dcGymListing))
                _Favorites = value
            End Set
        End Property

        <DataMember(IsRequired:=True)>
        Public Property Status() As dcOperationStatus
            Get
                Return _Status
            End Get
            Set(ByVal value As dcOperationStatus)
                _Status = value
            End Set
        End Property


        Public Sub New()
        End Sub



    End Class



End Namespace