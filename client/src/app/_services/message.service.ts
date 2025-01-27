import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append("Container", container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + "messages/thread/" + username);
  }

  sendMessage(username: string, content: string) {
    if (!username || !content) {
      console.error('Invalid username or content:', { username, content });
      return throwError(() => new Error('Recipient username or content is missing.'));
    }
  
    return this.http.post<Message>(this.baseUrl + "messages", { recipientUsername: username, content }).pipe(
      catchError((error) => {
        console.error('Error sending message:', error);
        return throwError(() => error);
      })
    );
  }
  

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
