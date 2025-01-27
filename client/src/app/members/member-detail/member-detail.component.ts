import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    // Access the resolved data from the route
    this.route.data.subscribe(data => {
      this.member = data['member'];  // The resolved member data is available here
      if (this.member) {
        this.galleryImages = this.getImages();
      } else {
        console.error('No member data found.');
      }
    });

    // Handling tab selection from query params
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

  // Method to get images for the gallery
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
  

  // Method to load messages
  loadMessages() {
    this.messageService.getMessageThread(this.member.userName).subscribe(messages => {
      this.messages = messages;
    });
  }

  // Method to select a tab by its index
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  // Method to handle tab activation and load messages when needed
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }
}
