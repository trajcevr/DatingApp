import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;
  user: User;

  constructor(
    public messageService: MessageService,
    private accountService: AccountService
  ) { }

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe(user => {
      if (user) {
        this.user = user;
        // if (this.username) {
        //   this.messageService.createHubConnection(this.user, this.username);
        // }
      }
    });
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  sendMessage() {
    if (!this.messageForm || !this.username || !this.messageContent) {
      console.error('Form, username, or message content is missing.');
      return;
    }

    this.messageService.sendMessage(this.username, this.messageContent).then(
      () => {
        this.messageForm.reset();
      },
      (error) => {
        console.error('Error sending message:', error);
      }
    );
  }
}
