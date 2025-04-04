Imports System.Resources.ResXFileRef
Imports System.Windows.Forms.Design.Behavior
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip
Imports Microsoft.Identity.Client

Public Class MyMsgLocalAuth

    Private ReadOnly MyConverter As MyMsgBoxConverter
    Private ReadOnly Uuid As Integer = GetUuid()

    Private Account As IAccount

    Public Sub New(Converter As MyMsgBoxConverter)
        Try
            InitializeComponent()
            Btn1.Name = Btn1.Name & GetUuid()
            Btn2.Name = Btn2.Name & GetUuid()
            Btn3.Name = Btn3.Name & GetUuid()
            MyConverter = Converter
            ShapeLine.StrokeThickness = GetWPFSize(1)
            Init()
        Catch ex As Exception
            Log(ex, "登录弹窗初始化失败", LogLevel.Hint)
        End Try
    End Sub

    Private Sub Load(sender As Object, e As EventArgs) Handles MyBase.Loaded
        Try
            '动画
            Opacity = 0
            AniStart(AaColor(FrmMain.PanMsg, Grid.BackgroundProperty, If(MyConverter.IsWarn, New MyColor(140, 80, 0, 0), New MyColor(90, 0, 0, 0)) - FrmMain.PanMsg.Background, 200), "PanMsg Background")
            AniStart({
                AaOpacity(Me, 1, 120, 60),
                AaDouble(Sub(i) TransformPos.Y += i, -TransformPos.Y, 300, 60, New AniEaseOutBack(AniEasePower.Weak)),
                AaDouble(Sub(i) TransformRotate.Angle += i, -TransformRotate.Angle, 300, 60, New AniEaseOutFluent(AniEasePower.Weak))
            }, "MyMsgBox " & Uuid)
            '记录日志
            Log("[Control] 登录弹窗：" & LabTitle.Text & vbCrLf & LabCaption.Text)
        Catch ex As Exception
            Log(ex, "登录弹窗加载失败", LogLevel.Hint)
        End Try
    End Sub

    Private Sub Close()
        '动画
        AniStart({
            AaCode(
            Sub()
                If Not WaitingMyMsgBox.Any() Then
                    AniStart(AaColor(FrmMain.PanMsg, Grid.BackgroundProperty, New MyColor(0, 0, 0, 0) - FrmMain.PanMsg.Background, 200, Ease:=New AniEaseOutFluent(AniEasePower.Weak)))
                End If
            End Sub, 30),
            AaOpacity(Me, -Opacity, 80, 20),
            AaDouble(Sub(i) TransformPos.Y += i, 20 - TransformPos.Y, 150, 0, New AniEaseOutFluent),
            AaDouble(Sub(i) TransformRotate.Angle += i, 6 - TransformRotate.Angle, 150, 0, New AniEaseInFluent(AniEasePower.Weak)),
            AaCode(Sub() CType(Parent, Grid).Children.Remove(Me), , True)
        }, "MyMsgBox " & Uuid)
    End Sub

    Public Sub Btn1_Click() Handles Btn1.Click
        Finished(Account)
    End Sub
    Public Sub Btn2_Click() Handles Btn2.Click
        Finished(New ThreadInterruptedException)
    End Sub
    Public Sub Btn3_Click() Handles Btn3.Click
        Finished(New ThreadInterruptedException)
    End Sub

    Private Sub Finished(Result As Object)
        If MyConverter.IsExited Then Return
        MyConverter.IsExited = True
        MyConverter.Result = Result
        RunInUi(AddressOf Close)
        Thread.Sleep(200)
        FrmMain.ShowWindowToTop()
    End Sub

    Private Sub Init()
        '设置 UI
        LabTitle.Text = "登录 Minecraft"
        LabCaption.Text = $"是否以 {Account.Username} 登录？"
    End Sub
End Class
