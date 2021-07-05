import { Component, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpEventType, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, Observer } from 'rxjs';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { FormControl } from '@angular/forms';

export interface S3Object {
  key: string;
  lastmodified: Date;
  bucket: string;
  size: number;
  type: string
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  public progress: number = 0;
  public messages: String[] = [];
  msgCtrl = new FormControl('');


  public httpError: boolean = false;
  public loading: boolean = false;
  public error: any = undefined;

  columnDefs = [
    { field: 'key', sortable: true, filter: true, checkboxSelection: true },
    { field: 'lastmodified', sortable: true, filter: true },
    { field: 'size', sortable: true, filter: true },
    { field: 'type', sortable: true, filter: true }
  ];

  public rowData: S3Object[] = [];
  public newData: S3Object[] = [];

  @ViewChild('fileupload') fileUpload : any;
  @ViewChild('agGrid') AgGridAngular : any;

  @Output() public onUploadFinished = new EventEmitter();

  PostObserver = {
    next: (data) => {
      this.newData = data.body;
      this.loading = false;
      this.httpError = false;
    },
    error: (err: HttpErrorResponse) => {
      this.error = err;
      this.loading = false;
      this.httpError = true;
    },
    complete: () => {
      console.log('PostObserver got a complete notification');
      this.GetObjectsInBucket('test-208561-pricex-pricex-s3');
      this.loading = false;
      this.httpError = false;
      if (this.newData != null && this.newData.length > 0)
      {
        this.rowData.push(...this.newData);
      }
    },
  } as Observer<any>;

  getObserver = {
    next: (data) => {
      this.rowData = data.body;
      this.loading = false;
      this.httpError = false;
    },
    error: (err: HttpErrorResponse) => {
      this.error = err;
      this.loading = false;
      this.httpError = true;
    },
    complete: () => {
      console.log('GetObserver got a complete notification');
      this.loading = false;
      this.httpError = false;
    },
  } as Observer<any>;

  DeleteObserver = {
    next: (data) => {
      this.loading = false;
      this.httpError = false;
    },
    error: (err: HttpErrorResponse) => {
      this.error = err;
      this.loading = false;
      this.httpError = true;
    },
    complete: () => {
      console.log('DeleteObserver got a complete notification');
      this.loading = false;
      this.httpError = false;
      this.GetObjectsInBucket('test-208561-pricex-pricex-s3');
    },
  } as Observer<any>;

  constructor(private http: HttpClient ) { }

  ngOnInit() {

    this.messages = [];

    this.messages.push("initial message");

    this.GetObjectsInBucket('test-208561-pricex-pricex-s3');

  }


  public GetObjectsInBucket(bucket: string)
  {
    this.http.get<S3Object[]>('https://localhost:5001/api/s3/' + bucket, { observe: 'response'}).subscribe(this.getObserver);
  }

  public uploadFile(files : FileList | null)
  {
    if ( files === null || files.length < 1) {
      return;
    }

    const formData = new FormData();

    Array.from(files).map((file, index) => {
      return formData.append('file ' + index, file, file.name);
    });


    this.http.post('https://localhost:5001/api/s3', formData, { observe: 'response' }).subscribe(this.PostObserver);

    this.fileUpload.nativeElement.value = "";

  }

  DeleteSelectedRows(): void
  {
    const selectedNodes = this.AgGridAngular.api.getSelectedNodes();
    const selectedData = selectedNodes.map( (node : any) => node.data);

    for (const ob of selectedData)
    {
      const url = 'https://localhost:5001/api/s3/test-208561-pricex-pricex-s3/' + ob.key.replace('/', '@');
      this.http.delete(url).subscribe(this.DeleteObserver);

    }

  }


}
