import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }


  getMembers() {
    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user')??"")?.token
      })
    }
    console.log("local storage je: <start>");
    console.log(localStorage.getItem('user'));
    console.log("local storage je: </end>");
    return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  }

  getMember(username: string) {

    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user')??"")?.token
      })
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username, httpOptions);
  }

}

