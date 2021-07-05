import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettings } from '../common/AppSettings';


@Injectable({
  providedIn: 'root'
})
export class AWSHttpService {
  constructor(private httpClient: HttpClient) { }

  public GetObjectsInBucket(bucket: string) {
    const url = AppSettings.APIBASEURL + bucket + "/";
    return this.httpClient.get<any>(url, { observe: 'response' });
  }

  public DeleteObjectInBucket(bucket: string, key: string) {
    const url = AppSettings.APIBASEURL + bucket + "/" + key;
    return this.httpClient.delete(url, { observe: 'response' });
  }

}
