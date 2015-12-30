Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcCheckinsResponse")> _
    Public Class dcCheckinsResponse
        Private _Checkins As New List(Of dcCheckin)
        Private _Status As dcOperationStatus

        <DataMember(IsRequired:=True)>
        Public Property Checkins() As List(Of dcCheckin)
            Get
                Return _Checkins
            End Get
            Set(ByVal value As List(Of dcCheckin))
                _Checkins = value
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