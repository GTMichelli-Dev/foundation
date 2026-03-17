Public Class Com_Error

    Public Sub New(ByVal Message As String, Errormsg As String)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Label1.Text = Message
        Me.Label2.Text = Errormsg
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class