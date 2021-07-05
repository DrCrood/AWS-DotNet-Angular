import { TestBed } from '@angular/core/testing';

import { AWSHttpService } from './awshttp.service';

describe('AWSHttpService', () => {
  let service: AWSHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AWSHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
