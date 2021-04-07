﻿Imports System.Globalization

Public Class AddBookUserControl
    Private selectedBook As BookDetailsDTO
    Private classifications As List(Of ClassificationDTO)
    Private classificationNames As New List(Of String)
    Private provider As CultureInfo = CultureInfo.InvariantCulture

    Private authors As New List(Of AuthorDTO)
    Private copies As New List(Of BookCopyDTO)
    Private prevCopies As New List(Of BookCopyDTO)
    Private imgFlName As String = ""
    Private status As New List(Of String)({"Available", "Not Available"})

    Private Sub AddBook_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        classifications = CategoryController.getCategories()
        classifications.Sort(Function(x, y) x.id.CompareTo(y.id))
        For Each classificationDto As ClassificationDTO In classifications
            classificationNames.Add(classificationDto.name)
        Next
        classificationCmbBx.DataSource = classificationNames
        ' copiesDataGridView.Columns.Data
        ' populate(4)
        ' setAuthors()
        empty()
    End Sub

    '  EVENT HANDLERS

    ' books
    Private Sub addBookBtn_Click(sender As Object, e As EventArgs) Handles addHoverPcBx.Click

        Dim newBook As New BookDetailsDTO

        newBook.title = titleTxtBx.Text
        newBook.isbn = isbnTxtBx.Text
        ' TODO: check if same to the size of the copydatagridview
        newBook.language = languageTxtBx.Text
        newBook.summary = summaryRichTxtBx.Text
        newBook.edition = editionTxtBx.Text

        If publishedDatePicker.Checked Then
            newBook.publishedDate = publishedDatePicker.Value.ToString("yyyy-MM-dd")
        End If

        If String.Compare(publisherNameTxtBx.Text.Trim, "") <> 0 Then
            newBook.publisherName = publisherNameTxtBx.Text.Trim
        End If
        If String.Compare(publisherAddrTxtBx.Text.Trim, "") <> 0 Then
            newBook.publisherAddress = publisherAddrTxtBx.Text
        End If


        If String.Compare(copyrightNameTxtBx.Text.Trim, "") <> 0 Then
            newBook.copyrightName = copyrightNameTxtBx.Text
        End If

        If copyrightYearDTPckr.Checked Then
            newBook.copyrightYear = copyrightYearDTPckr.Value.ToString("yyyy")
        End If

        newBook.categoryId = classificationCmbBx.SelectedIndex + 1
        newBook.shelfName = shelfTxtBx.Text

        Dim hasImg As Boolean = False
        Dim ext = ""
        ' image
        If String.Compare(imgFlName, "") <> 0 Then
            hasImg = True
            Dim strt = imgFlName.LastIndexOf(".")
            ext = imgFlName.Substring(strt, imgFlName.Length - strt)
            newBook.image = "new" + ext ' TODO: set the server to set its image attrs to its id
        Else
            newBook.image = "empty"
        End If

        setAuthors()
        newBook.authors = authors
        setCopies()
        newBook.copies = copies
        newBook.quantity = copies.Count
        Dim response = BookController.addNewBook(newBook)
        selectedBook = newBook

        If hasImg Then
            uploadImage(response + ext)  ' upload the image
        End If

        MessageBox.Show("Successfully added book!")
        empty()
        populate(response)
    End Sub


    Private Sub savePcBx_Click(sender As Object, e As EventArgs) Handles saveHoverPcBx.Click

        Dim attrs As New Dictionary(Of String, String)
        If String.Compare(selectedBook.title.Trim, selectedBook.title) <> 0 Then
            attrs.Add("title", selectedBook.title.Trim)
        End If
        If String.Compare(isbnTxtBx.Text.Trim, selectedBook.isbn) <> 0 Then
            attrs.Add("isbn", isbnTxtBx.Text.Trim)
        End If
        If String.Compare(languageTxtBx.Text.Trim, selectedBook.language) <> 0 Then
            attrs.Add("language", languageTxtBx.Text.Trim)
        End If
        If String.Compare(summaryRichTxtBx.Text.Trim, selectedBook.summary) <> 0 Then
            attrs.Add("summary", summaryRichTxtBx.Text.Trim)
        End If
        If String.Compare(editionTxtBx.Text.Trim, selectedBook.edition.ToString) <> 0 Then
            attrs.Add("edition", editionTxtBx.Text.Trim)
        End If

        If publishedDatePicker.Checked Then
            If String.Compare(publishedDatePicker.Value.ToString("yyyy-MM-dd"), selectedBook.publishedDate) <> 0 Then
                attrs.Add("publishedDate", publishedDatePicker.Value.ToString("yyyy-MM-dd"))
            End If
        Else
            attrs.Add("publishedDate", Nothing)
        End If

        If String.Compare(publisherNameTxtBx.Text.Trim, selectedBook.publisherName) <> 0 Then
            If String.Compare(publisherNameTxtBx.Text.Trim, "") <> 0 Then
                attrs.Add("publisherName", publisherNameTxtBx.Text.Trim)
            Else
                attrs.Add("publisherName", Nothing)
            End If
        End If
        If String.Compare(publisherAddrTxtBx.Text.Trim, selectedBook.publisherAddress) <> 0 Then
            If String.Compare(publisherAddrTxtBx.Text.Trim, "") <> 0 Then
                attrs.Add("publisherAddress", publisherAddrTxtBx.Text.Trim)
            Else
                attrs.Add("publisherAddress", Nothing)
            End If
        End If
        If String.Compare(copyrightNameTxtBx.Text.Trim, selectedBook.copyrightName) <> 0 Then
            If String.Compare(copyrightNameTxtBx.Text.Trim, "") <> 0 Then
                attrs.Add("copyrightName", copyrightNameTxtBx.Text.Trim)
            Else
                attrs.Add("copyrightName", Nothing)
            End If
        End If
        If copyrightYearDTPckr.Checked Then
            If String.Compare(copyrightYearDTPckr.Value.ToString("yyyy"), selectedBook.copyrightYear) <> 0 Then
                attrs.Add("copyrightYear", copyrightYearDTPckr.Value.ToString("yyyy"))
            End If
        Else
            attrs.Add("copyrightYear", Nothing)
        End If

        If (classificationCmbBx.SelectedIndex + 1) <> selectedBook.categoryId Then
            ' TODO: check if the selected name exist
            attrs.Add("categoryId", classificationCmbBx.SelectedIndex + 1)
        End If

        If String.Compare(shelfTxtBx.Text.Trim, selectedBook.shelfName) <> 0 Then
            If String.Compare(shelfTxtBx.Text.Trim, "") <> 0 Then
                attrs.Add("shelfName", shelfTxtBx.Text.Trim)
            Else
                attrs.Add("shelfName", Nothing)
            End If
        End If

        ' check if there is changes in authors
        ' authorsDataGrid.ClearSelection()
        authorsDataGrid.CurrentCell = Nothing
        setAuthors()
        Dim updateAuthor As Boolean = False
        If authors.Count <> selectedBook.authors.Count Then
            updateAuthor = True
        Else
            For Each author As AuthorDTO In authors
                If Not selectedBook.authors.Contains(author) Then
                    updateAuthor = True
                    Exit For
                End If
            Next
        End If


        ' check if there is changes in copies
        copiesDataGridView.CurrentCell = Nothing
        setCopies()
        Dim updateCopies As Boolean = False
        If copies.Count <> prevCopies.Count Then
            updateCopies = True
        Else
            For Each copy As BookCopyDTO In copies
                If Not prevCopies.Contains(copy) Then
                    updateCopies = True
                    Exit For
                End If
            Next
        End If

        ' image
        Dim uploadImg As Boolean = False
        Dim removeImg As Boolean = False
        Dim ext = ""

        If String.Compare(imgFlName, "") <> 0 Then
            uploadImg = True
            Dim strt = imgFlName.LastIndexOf(".")
            ext = imgFlName.Substring(strt, imgFlName.Length - strt)
            selectedBook.image = selectedBook.bookId.ToString + ext
            attrs.Add("image", selectedBook.image)
        Else
            If String.Compare(selectedBook.image, "empty") <> 0 Then
                removeImg = True
                attrs.Add("image", "empty")
            End If
        End If

        If attrs.Count <> 0 Then
            ' call request for update author
            BookController.updateBook(selectedBook.bookId, attrs)
        End If

        If updateAuthor Then
            ' request for update author possible on another thread
            BookController.updateAuthorOfBook(selectedBook.bookId, authors)
        End If

        If updateCopies Then
            CopyController.updateCopiesOfBook(selectedBook.bookId, copies)
        End If

        If uploadImg Then
            uploadImage(selectedBook.image)  ' upload the image
        ElseIf removeImg Then
            ImageController.removeImage(selectedBook.image)
        End If
        MessageBox.Show("Successfully updated!")
        populate(selectedBook.bookId)
    End Sub

    Private Sub cancelHoverBtn_Click(sender As Object, e As EventArgs) Handles cancelHoverPcBx.Click
        ' TODO: go back to previous selected tab?
    End Sub

    ' authors
    Private Sub setAuthors()
        authors.Clear()
        'For Each row As DataGridViewRow In authorsDataGrid.Rows - 1
        For idx As Integer = 0 To authorsDataGrid.Rows.Count - 2
            Dim newAuthor As New AuthorDTO
            Dim f_name = authorsDataGrid.Item(0, idx).Value.ToString().Trim
            Dim m_name = authorsDataGrid.Item(1, idx).Value.ToString().Trim
            Dim l_name = authorsDataGrid.Item(2, idx).Value.ToString().Trim
            Dim attrs As New Dictionary(Of String, String)

            If f_name.Equals("") AndAlso m_name.Equals("") AndAlso l_name.Equals("") Then
                Continue For
            End If

            If String.Compare(f_name, "") <> 0 Then
                attrs.Add("fName", f_name)
            End If

            If String.Compare(m_name, "") <> 0 Then
                attrs.Add("mName", m_name)
            End If

            If String.Compare(f_name, "") <> 0 Then
                attrs.Add("lName", l_name)
            End If

            Dim prevAuthor = AuthorController.findAuthorByName(attrs)

            If prevAuthor.id <> -1 Then
                newAuthor = prevAuthor
            Else
                newAuthor.id = -1
                newAuthor.f_name = f_name
                newAuthor.m_name = m_name
                newAuthor.l_name = l_name
            End If
            authors.Add(newAuthor)
        Next
        If authors.Count = 0 Then
            ' no author of the book
            authors.Add(New AuthorDTO(-1, Nothing, Nothing, Nothing))
        End If
    End Sub

    Private Sub setCopies()
        copies.Clear()

        For idx As Integer = 0 To copiesDataGridView.Rows.Count - 2
            Dim copy As New BookCopyDTO
            copy.id = -1
            copy.copy_num = copiesDataGridView.Item(0, idx).Value.ToString().Trim
            copy.status = copiesDataGridView.Item(1, idx).Value.ToString().Trim
            If copy.copy_num.Equals("") Then
                Continue For
            End If
            If copies.Contains(copy) Then
                MessageBox.Show("Failed: Instances contain same Copy #")
            Else
                copies.Add(copy)
            End If
        Next
    End Sub


    ' image
    Private Sub addImgBtn_click(sender As Object, e As EventArgs) Handles bkPicBx.Click
        Dim fileDialog = New OpenFileDialog()
        fileDialog.Filter = "Picture Files (*)|*.bmp;*.jpg;*.png"

        If fileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            imgFlName = fileDialog.FileName
            bkPicBx.Image = Image.FromFile(imgFlName)
            removeImgBtn.Visible = True
        End If
    End Sub

    Private Sub removeImgBtn_Click(sender As Object, e As EventArgs) Handles removeImgBtn.Click
        bkPicBx.Image = My.Resources.default_book
        imgFlName = ""
        removeImgBtn.Visible = False
    End Sub


    ' HELPER FUNCTIONS/SUBS
    Private Sub populate(bookId As String)

        selectedBook = BookController.getBook(bookId)
        titleTxtBx.Text = selectedBook.title
        isbnTxtBx.Text = selectedBook.isbn
        languageTxtBx.Text = selectedBook.language
        editionTxtBx.Text = selectedBook.edition
        'numAvailableLbl.Text = selectedBook.numAvailable

        publisherNameTxtBx.Text = selectedBook.publisherName
        publisherAddrTxtBx.Text = selectedBook.publisherAddress
        If selectedBook.publishedDate <> Nothing Then
            publishedDatePicker.Value = Date.ParseExact(selectedBook.publishedDate, "yyyy-MM-dd", provider)
        Else
            publishedDatePicker.Value = publishedDatePicker.MinDate
            publishedDatePicker.Checked = False
        End If

        classificationCmbBx.SelectedIndex = selectedBook.categoryId - 1
        copyrightNameTxtBx.Text = selectedBook.copyrightName
        If selectedBook.copyrightYear <> 0 Then
            copyrightYearDTPckr.Value = Date.ParseExact(selectedBook.copyrightYear, "yyyy", provider)

        Else
            copyrightYearDTPckr.Value = copyrightYearDTPckr.MinDate
            copyrightYearDTPckr.Checked = False
        End If

        shelfTxtBx.Text = selectedBook.shelfName
        summaryRichTxtBx.Text = selectedBook.summary
        ' authors
        authors = New List(Of AuthorDTO)
        For Each author As AuthorDTO In selectedBook.authors
            authors.Add(New AuthorDTO(author.id, author.f_name, author.m_name, author.l_name))
            ' removeAuthorBtn.Visible = True
        Next
        populateAuthors()

        ' load copies
        ' TODO: separate thread
        copies = CopyController.getCopies(selectedBook.bookId)
        prevCopies.AddRange(copies)  ' created a copy to check if there is difference when saving later on 
        populateCopies()
        quantityLbl.Text = copiesDataGridView.Rows.Count - 1
        If String.Compare(selectedBook.image, "empty") <> 0 Then
            ' TODO: separate thread or process
            bkPicBx.Image = getImage(selectedBook.image)
            removeImgBtn.Visible = True
        Else
            bkPicBx.Image = My.Resources.default_book
            removeImgBtn.Visible = False
        End If
        addPcBx.Visible = False
        savePcBx.Visible = True
    End Sub

    Private Sub populateAuthors()
        authorsDataGrid.Rows.Clear()
        For Each author In authors
            authorsDataGrid.Rows.Add({author.f_name, author.m_name, author.l_name})
        Next
    End Sub

    Private Sub populateCopies()
        copiesDataGridView.Rows.Clear()
        For Each copy In copies
            copiesDataGridView.Rows.Add({copy.copy_num, copy.status})
            'copiesDataGridView.Rows.Add({copy.copy_num, "Not Available"})
        Next
    End Sub

    Private Sub uploadImage(flNm As String)
        bkPicBx.Image.Dispose()
        Dim res As String = ImageController.uploadImage(imgFlName, flNm)
        bkPicBx.Image = Image.FromFile(imgFlName)
    End Sub

    Private Sub loadImage(flNm As String)
        Dim imgRetuned = ImageController.getImage(flNm)
        bkPicBx.Image = imgRetuned
    End Sub

    Private Sub empty()

        selectedBook = Nothing
        titleTxtBx.Text = String.Empty
        isbnTxtBx.Text = String.Empty
        languageTxtBx.Text = String.Empty
        editionTxtBx.Text = String.Empty

        publisherNameTxtBx.Text = String.Empty
        publisherAddrTxtBx.Text = String.Empty

        publishedDatePicker.Value = Date.Now()
        publishedDatePicker.Checked = True

        classificationCmbBx.SelectedIndex = 0
        copyrightNameTxtBx.Text = String.Empty
        copyrightYearDTPckr.Value = Date.Now()
        copyrightYearDTPckr.Checked = True

        shelfTxtBx.Text = String.Empty
        summaryRichTxtBx.Text = String.Empty

        authors.Clear()
        authorsDataGrid.Rows.Clear()
        copies.Clear()
        copiesDataGridView.Rows.Clear()
        copiesDataGridView.Rows.Add({1, status.Item(0)})

        bkPicBx.Image = My.Resources.default_book
        removeImgBtn.Visible = False


        savePcBx.Visible = False
        addPcBx.Visible = True


        selectedBook = Nothing

        imgFlName = String.Empty

        populateAuthors()
    End Sub
    ' elements

    Private Sub authorsDataGrid_CellContentClick(sender As System.Object, e As DataGridViewCellEventArgs) _
                                           Handles authorsDataGrid.CellContentClick
        Dim senderGrid = DirectCast(sender, DataGridView)

        If TypeOf senderGrid.Columns(e.ColumnIndex) Is DataGridViewButtonColumn AndAlso
       e.RowIndex >= 0 Then
            If (e.ColumnIndex = 3) Then
                ' delete clicked
                authorsDataGrid.Rows.RemoveAt(e.RowIndex)
            End If
        End If
    End Sub
    Private Sub copiesDataGridView_CellContentClick(sender As System.Object, e As DataGridViewCellEventArgs) _
                                           Handles copiesDataGridView.CellContentClick
        Dim senderGrid = DirectCast(sender, DataGridView)

        If TypeOf senderGrid.Columns(e.ColumnIndex) Is DataGridViewButtonColumn AndAlso
       e.RowIndex >= 0 Then
            If (e.ColumnIndex = 2) Then
                ' delete clicked
                copiesDataGridView.Rows.RemoveAt(e.RowIndex)
                quantityLbl.Text = copiesDataGridView.Rows.Count() - 1
            End If
        End If
    End Sub

    Private Sub copiesDataGridView_CellChanged(sender As Object, e As EventArgs) Handles copiesDataGridView.CellLeave
        quantityLbl.Text = copiesDataGridView.Rows.Count - 1
    End Sub
    Private Sub copiesDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) Handles copiesDataGridView.DefaultValuesNeeded
        ' copiesDataGridView.Rows.Item(1).Cells.Item(1).Cell
        e.Row.Cells("statusCol").Value = "Available"
    End Sub
    Private Sub cancelPcBx_Hover(sender As Object, e As EventArgs) Handles cancelPcBx.MouseHover
        cancelHoverPcBx.Visible = True
        cancelPcBx.Visible = False
    End Sub
    Private Sub cancelHoverPcBx_Hover(sender As Object, e As EventArgs) Handles cancelHoverPcBx.MouseLeave
        cancelHoverPcBx.Visible = False
        cancelPcBx.Visible = True
    End Sub

    Private Sub savePcBx_Hover(sender As Object, e As EventArgs) Handles savePcBx.MouseHover
        saveHoverPcBx.Visible = True
        savePcBx.Visible = False
    End Sub

    Private Sub saveHoverPcBx_Hover(sender As Object, e As EventArgs) Handles saveHoverPcBx.MouseLeave
        saveHoverPcBx.Visible = False
        savePcBx.Visible = True
    End Sub
    Private Sub addPcBx_Hover(sender As Object, e As EventArgs) Handles addPcBx.MouseHover
        addHoverPcBx.Visible = True
        addPcBx.Visible = False
    End Sub

    Private Sub addHoverPcBx_Hover(sender As Object, e As EventArgs) Handles addHoverPcBx.MouseLeave
        addHoverPcBx.Visible = False
        addPcBx.Visible = True
    End Sub

    Private Sub layoutPanel_Paint(sender As Object, e As PaintEventArgs) Handles layoutPanel.Paint
        'timerClearDataGrid.Enabled = True
    End Sub
End Class