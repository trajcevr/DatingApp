import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User;

  constructor(
    public presence: PresenceService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private accountService: AccountService,
    private router: Router
  ) {
    this.accountService.currentUser$
      .pipe(
        take(1) 
      )
      .subscribe({
        next: (user) => this.user = user
      });
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data['member'];  
      if (this.member) {
        this.galleryImages = this.getImages();
      } else {
        console.error('No member data found.');
      }
    
    });


    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });

    // Gallery options configuration
    this.galleryOptions = [
      {
        width: '600px',
        height: '400px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    
  }

  getImages(): NgxGalleryImage[] {
    if (!this.member || !this.member.photos || this.member.photos.length === 0) {
      console.warn('No photos available, using default image.');
      return [{
        small: './assets/default-image.png',
        medium: './assets/default-image.png',
        big: './assets/default-image.png'
      }];
    }

    const imageUrls = this.member.photos.map(photo => ({
      small: photo?.url ? photo.url : './assets/default-image.png',
      medium: photo?.url ? photo.url : './assets/default-image.png',
      big: photo?.url ? photo.url : './assets/default-image.png'
    }));

    return imageUrls;
  }


  loadMessages() {
    this.messageService.getMessageThread(this.member.userName).subscribe(messages => {
      this.messages = messages;
      console.log(messages);
    });
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
      if (this.messages.length === 0) {
        this.loadMessages(); 
      }
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }
  

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
