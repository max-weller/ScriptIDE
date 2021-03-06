﻿Public Class frmTB_openedFiles

  Dim WithEvents dd As iGDragDropManager


  Public Overrides Function GetPersistString() As String
    Return tbPrefix + "OpenedFiles"
  End Function
  Shadows Sub Show()
    'hier wird das Fenster ins Dockpanel eingefügt
    'ohne diesen Aufruf wäre es eine normale Form:
    cls_IDEHelper.GetSingleton().BeforeShowAddinWindow(Me.GetPersistString(), Me)
    MyBase.Show()
  End Sub


  Private Sub frmTB_openedFiles_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    dd = New iGDragDropManager()
    dd.AllowMoveWithinOneGrid = True
    dd.AllowMove = True
    dd.Manage(IGrid1, True, True)
  End Sub

  Private Sub IGrid1_CellMouseDown(ByVal sender As Object, ByVal e As TenTec.Windows.iGridLib.iGCellMouseDownEventArgs) Handles IGrid1.CellMouseDown

  End Sub

  Sub onRowClicked(ByVal idx As Integer)
    Dim pnl = IGrid1.Rows(idx).Tag 'getDeserializedDockContent(IGrid1.Rows(e.RowIndex).Tag)
    If pnl.IsDisposed Then createOpenedTabList() : Exit Sub

    pnl.show()

  End Sub

  Private Sub IGrid1_CellMouseUp(ByVal sender As Object, ByVal e As TenTec.Windows.iGridLib.iGCellMouseUpEventArgs) Handles IGrid1.CellMouseUp
    On Error Resume Next
    Select Case e.Button
      Case MouseButtons.Left
        '  setActRtfBox(IGrid1.Rows(e.RowIndex).Tag)
        '  MAIN.tssl_Filename.Text = IGrid1.Rows(e.RowIndex).Key + "   " + IGrid1.Rows(e.RowIndex).Tag.getFileTag()
        onRowClicked(e.RowIndex)

      Case MouseButtons.Right, Windows.Forms.MouseButtons.Middle
        closeTab(IGrid1.Rows(e.RowIndex).Tag)
        createOpenedTabList()

        'Dim idc As IDockContentForm = IGrid1.Rows(e.RowIndex).Tag
        'idc.Parameters("ColorBoxes") = "#ff0000,#00ff00"
    End Select
  End Sub

  Private Sub IGrid1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IGrid1.Click

  End Sub

  Private Sub dd_DragDropDone(ByVal MovedGridRow As TenTec.Windows.iGridLib.iGRow) Handles dd.DragDropDone
    Dim frm As DockContent = MovedGridRow.Tag
    Dim pane = frm.DockHandler.Pane
    Dim targetIndex = MovedGridRow.Index
    If pane.Contents.IndexOf(frm) < targetIndex Then
      targetIndex += 1
    End If
    If targetIndex >= pane.Contents.Count - 1 Then targetIndex = pane.Contents.Count - 1

    'TODO DockPanel-Bug beheben: man kann kein Element ans Ende schieben
    frm.DockHandler.Pane.SetContentIndex(frm, targetIndex)
    frm.Show()
  End Sub

  Private Sub txtSearch_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSearch.GotFocus
    If txtSearch.Text = "suche..." Then txtSearch.Text = ""
  End Sub

  Private Sub txtSearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSearch.KeyDown
    If e.KeyCode = Keys.Enter Then
      e.SuppressKeyPress = True
      e.Handled = True

      'suchen
      Dim such As String = txtSearch.Text
      If such.Contains("^") = False And such.Contains("$") = False And such.Contains("*") = False And such.Contains("?") = False Then _
      such = "*" + such + "*"

      Dim startItem As Integer = IGrid1.CurRow.Index + 1
startOver:
      For i = startItem To IGrid1.Rows.Count - 1
        Dim str As String = "^" + IGrid1.Cells(i, 0).Value + "$"
        If str Like such Then
          IGrid1.Rows(i).EnsureVisible()
          ' IGrid1.SetCurRow(i)
          onRowClicked(i)
          Exit Sub
        End If
      Next
      If startItem > 0 Then startItem = 0 : GoTo startOver

      Beep()

    End If
  End Sub

  Private Sub txtSearch_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSearch.LostFocus
    If txtSearch.Text = "" Then txtSearch.Text = "suche..."
  End Sub

  Private Sub txtSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.TextChanged
    If txtSearch.Text = "" Or txtSearch.Text = "suche..." Then
      'zurücksetzen

      For i = 0 To IGrid1.Rows.Count - 1
        IGrid1.Rows(i).BackColor = Color.Transparent
      Next

    Else
      'suchen
      Dim such As String = txtSearch.Text
      If such.Contains("^") = False And such.Contains("$") = False And such.Contains("*") = False And such.Contains("?") = False Then _
      such = "*" + such + "*"

      Dim first As Boolean = True
      For i = 0 To IGrid1.Rows.Count - 1
        Dim str As String = "^" + IGrid1.Cells(i, 0).Value + "$"
        If str Like such Then
          IGrid1.Rows(i).BackColor = Color.BurlyWood
          If first Then
            IGrid1.Rows(i).EnsureVisible()
            first = False
          End If
        Else
          IGrid1.Rows(i).BackColor = Color.Transparent
        End If


      Next

    End If
  End Sub

  Private Sub IGrid1_CustomDrawCellForeground(ByVal sender As Object, ByVal e As TenTec.Windows.iGridLib.iGCustomDrawCellEventArgs) Handles IGrid1.CustomDrawCellForeground
    Dim idc As IDockContentForm = IGrid1.Rows(e.RowIndex).Tag
    Dim bb As String = idc.Parameters("ColorBoxes")
    If String.IsNullOrEmpty(bb) = False Then
      Dim boxes() As String = Split(bb, ",")
      For i = 0 To boxes.Length - 1
        e.Graphics.FillRectangle(New SolidBrush(ColorTranslator.FromHtml(boxes(i))), e.Bounds.Width - (i + 1) * 15, e.Bounds.Y + 1, 13, 13)
      Next
    End If
    e.Graphics.DrawImage(imlIgrid.Images(IGrid1.Cells(e.RowIndex, 0).ImageIndex), 1, e.Bounds.Y)
    e.Graphics.DrawString(IGrid1.Cells(e.RowIndex, 0).Value, IGrid1.Font, If(e.RowSelected, Brushes.White, Brushes.Black), 17, e.Bounds.Y + 1)

  End Sub

End Class