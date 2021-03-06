﻿Imports SweatWorksData

Namespace DataContracts

    <Serializable()> _
    <DataContract(Name:="dcLoginRequest")> _
    Public Class dcLoginRequest
        Private _Email As String
        Private _Password As String
        Private _FacebookID As String
        Private _FirstName As String
        Private _LastName As String

        <DataMember(IsRequired:=True)>
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property Password() As String
            Get
                Return _Password
            End Get
            Set(ByVal value As String)
                _Password = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property FacebookID() As String
            Get
                Return _FacebookID
            End Get
            Set(ByVal value As String)
                _FacebookID = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property FirstName() As String
            Get
                Return _FirstName
            End Get
            Set(ByVal value As String)
                _FirstName = value
            End Set
        End Property

        <DataMember(IsRequired:=False)>
        Public Property LastName() As String
            Get
                Return _LastName
            End Get
            Set(ByVal value As String)
                _LastName = value
            End Set
        End Property


        Public Sub New()
        End Sub

        Public Overrides Function toString() As String
            Return "email: " + Email + " password: " + Password + " FacebookID: " + FacebookID
        End Function

    End Class

End Namespace