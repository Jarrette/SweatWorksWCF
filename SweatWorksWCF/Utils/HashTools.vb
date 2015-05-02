Imports System.Security.Cryptography
Imports System.Numerics

Public Class HashTools

    Const SALT_BYTE_SIZE As Integer = 24
    Const HASH_BYTE_SIZE As Integer = 24
    Const PBKDF2_ITERATIONS As Integer = 1000
    Const ITERATION_INDEX As Integer = 0
    Const SALT_INDEX As Integer = 1
    Const PBKDF2_INDEX As Integer = 2


    Public Shared Function CreateHash(ByVal strPassword As String) As String
        'create a random salt
        Dim csprng As New RNGCryptoServiceProvider
        Dim bytSalt(SALT_BYTE_SIZE) As Byte
        csprng.GetBytes(bytSalt)

        ' Hash the password and encode the parameters
        Dim bytHash As Byte() = PBKDF2(strPassword, bytSalt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE)
        Return PBKDF2_ITERATIONS.ToString + ":" + Convert.ToBase64String(bytSalt) + ":" + Convert.ToBase64String(bytHash)
    End Function

    ''' <summary>
    ''' Validates a password given a hash of the correct one.
    ''' </summary>
    ''' <param name="strTestPassword">The password to check.</param>
    ''' <param name="strCorrectHash">A hash of the correct password.</param>
    ''' <returns>True if the password is correct. False otherwise.</returns>
    ''' <remarks></remarks>
    Public Shared Function ValidatePassword(ByVal strTestPassword As String, ByVal strCorrectHash As String) As Boolean
        ' Extract the parameters from the hash
        Dim delimiter As Char() = {":"}
        Dim split() As String = strCorrectHash.Split(delimiter)
        Dim intIterations As Integer = Int32.Parse(split(ITERATION_INDEX))
        Dim bytSalt() As Byte = Convert.FromBase64String(split(SALT_INDEX))
        Dim bytHash() As Byte = Convert.FromBase64String(split(PBKDF2_INDEX))
        Dim bytTestHash() As Byte = PBKDF2(strTestPassword, bytSalt, intIterations, bytHash.Length)
        Return SlowEquals(bytHash, bytTestHash)
    End Function


    ''' <summary>
    ''' Compares two byte arrays in length-constant time. This comparison
    ''' method is used so that password hashes cannot be extracted from
    ''' on-line systems using a timing attack and then attacked off-line.
    ''' </summary>
    ''' <param name="a">The first byte array.</param>
    ''' <param name="b">The second byte array.</param>
    ''' <returns>True if both byte arrays are equal. False otherwise.</returns>
    ''' <remarks></remarks>
    Protected Shared Function SlowEquals(ByVal a() As Byte, ByVal b() As Byte) As Boolean
        Dim diff As UInteger = CUInt(a.Length) Xor CUInt(b.Length)
        Dim i As Integer = 0
        Do While i < a.Length AndAlso i < b.Length
            diff = diff Or CUInt(a(i) Xor b(i))
            i += 1
        Loop

        Return diff = 0
    End Function

    ''' <summary>
    ''' Computes the PBKDF2-SHA1 hash of a password.
    ''' </summary>
    ''' <param name="strPassword">The password to hash.</param>
    ''' <param name="bytSalt">The salt</param>
    ''' <param name="intIterations">The PBKDF2 iteration count</param>
    ''' <param name="intOutputBytes">The length of the hash to generate, in bytes.</param>
    ''' <returns>A hash of the password.</returns>
    ''' <remarks></remarks>
    Private Shared Function PBKDF2(ByVal strPassword As String, bytSalt As Byte(), ByVal intIterations As Integer, ByVal intOutputBytes As Integer) As Byte()
        Dim bytPbkdf2 As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(strPassword, bytSalt)
        bytPbkdf2.IterationCount = intIterations
        Return bytPbkdf2.GetBytes(intOutputBytes)
    End Function

End Class
