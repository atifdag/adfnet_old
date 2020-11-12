import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';

export interface DialogData {
  title: string;
  message: string;
}

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})


export class ConfirmDialogComponent implements OnInit {
  dialogData: DialogData;
  title: string;
  message: string;
  constructor(
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData

  ) { }

  ngOnInit(): void {
  }
  onConfirm(): boolean {
    this.dialogRef.close(true);
    // Close the dialog, return true
    return true;
  }

  onDismiss(): false {
    // Close the dialog, return false
    this.dialogRef.close(false);
    return false;
  }
}
