Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcGymsResponse")> _
    Public Class dcGymsResponse
        Private _Gyms As New List(Of dcGymResponse)
        Private _Status As dcOperationStatus

        <DataMember(IsRequired:=True)>
        Public Property Gyms() As List(Of dcGymResponse)
            Get
                Return _Gyms
            End Get
            Set(ByVal value As List(Of dcGymResponse))
                _Gyms = value
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