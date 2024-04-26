import { Component,OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = ' App!';

  constructor(private accountService:AccountService){}

  ngOnInit():void {
      this.setCurrentUser();
  }

//   getUsers()
//   {
//     this.http.get('https://localhost:5001/api/users').subscribe({
//       next:response=>this.users=response,
//       error:error=>console.log(error),
//       complete:()=>console.log('Request has Completed')
// });
//   }

  setCurrentUser(){
    const userString=localStorage.getItem('user');
    if(!userString) return;
    const user:User=JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}