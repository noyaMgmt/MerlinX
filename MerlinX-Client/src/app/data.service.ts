import { Injectable, OnInit, OnDestroy } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';

@Injectable({providedIn:'root'})

export class dataService {
  constructor(private httpClient:HttpClient){}
  flightsUpdated = new Subject<any>();

  getDataFromServer(from:Date,to:Date,selectedSource:string){
    return this.httpClient.get<any>('http://127.0.0.1:8081?dateFrom='+from.getTime()+'&dateTo='+to.getTime()+'&source='+selectedSource);
  }
  getSources(){
    return this.httpClient.get<any>('https://newsapi.org/v2/sources?apiKey=4ef9ec7a8516412095cb7fad0d40f699');
  }
}
