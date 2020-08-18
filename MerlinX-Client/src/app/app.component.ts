import { dataService } from './data.service';
import { Component, OnInit } from '@angular/core';
import { CloudData,CloudOptions  } from 'angular-tag-cloud-module';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit{

  // Word cloud parameters
  data: CloudData[] = [];
  options: CloudOptions = {
    width: 1,
    height: 1,
    overflow: false,
  };
  // Button parameters
  clicked = false;
  // Datepicker parameters
  maxDate:Date;
  startDate:Date = null;
  endDate:Date = null;
  // Source selector parameters
  sources:string[]=[];
  selectedSource:string;

  constructor(private dataService:dataService) {
    this.maxDate = new Date();
  }

  onClick()
  {
    this.clicked = true;
    this.dataService.getDataFromServer(this.startDate,this.endDate,this.selectedSource).subscribe(
      data => {
        let newData = [];
        for(let n of Object.entries(data).sort((a, b) =>  +a[1] > +b[1] ? -1:1))
        {
          newData.push({text:n[0],weight:+n[1]});
        }

        this.data = newData;
        this.clicked = false;
      }
    );
  }
  startDateSelected(event:MatDatepickerInputEvent<Date>){
    this.startDate = event.value;
  }
  endDateSelected(event:MatDatepickerInputEvent<Date>){
    this.endDate = event.value;
  }

  ngOnInit(){
    this.dataService.getSources().subscribe(data=>{
      this.sources= data.sources.map(x=>x.id);
      this.selectedSource = this.sources[0];
    });
  }

}
