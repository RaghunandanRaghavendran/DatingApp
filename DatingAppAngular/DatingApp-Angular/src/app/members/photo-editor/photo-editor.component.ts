import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
@Input() photos: Photo[];
@Output() getMemberPhotoChanged = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  baseUrl = environment.apiUrl;
  currentMain: Photo;


  constructor(private authService: AuthService, private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.intializeUploader();
  }

   fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

intializeUploader() {
  this.uploader = new FileUploader({
    url: this.baseUrl + 'users/' + this.authService.decodedtoken.nameid + '/photos',
    authToken: 'Bearer ' + localStorage.getItem('token'),
    isHTML5: true,
    allowedFileType: ['image'],
    removeAfterUpload: true,
    autoUpload: false,
    maxFileSize: 10 * 1024 * 1024
  });
  this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

  // After uploading the photo, it has to appear immediately
  this.uploader.onSuccessItem = (item, response, status, header) => {
    if ( response) {
      const res: Photo = JSON.parse(response);
      const photo = {
        id: res.id,
        url: res.url,
        uploadedTime: res.uploadedTime,
        description: res.description,
        isProfilePicture: res.isProfilePicture
      };
      this.photos.push(photo);
    }
  };
}

setMainPhoto(photo: Photo) {
  this.userService.setProfilePicture(this.authService.decodedtoken.nameid, photo.id).subscribe(
    () => {
      this.currentMain = this.photos.filter(p => p.isProfilePicture === true)[0];
      this.currentMain.isProfilePicture = false;
      photo.isProfilePicture = true;
      // this.getMemberPhotoChanged.emit(photo.url);
      this.authService.changeMemberPhoto(photo.url);
      // This is to make sure, once the page is refreshed, we still set the photourl to local storage
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    },
    error => {
     this.alertify.error('Error occured during setting the main');
    }
  );
}

deletePhoto(id: number) {

  this.alertify.confirm('Are you sure you want to delete the photo? ', () => {
    this.userService.deletePhoto(this.authService.decodedtoken.nameid, id).subscribe(() => {
     this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
     this.alertify.success('Photo has been deleted');
    }, error => {
      this.alertify.error('Failed to delete this photo');
    }

    );
  });
}

}
