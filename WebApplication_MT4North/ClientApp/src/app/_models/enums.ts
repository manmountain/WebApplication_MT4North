export enum ActivityPhase {
  CONCEPTUALIZATION = 0,
  VALIDATION = 1,
  DEVELOPMENT = 2,
  LAUNCH = 3
}

//CONCEPTUALIZATION = "Konceptualisering",
//  VALIDATION = "Konceptvalidering",
//  DEVELOPMENT = "Produktutveckling",
//  LAUNCH = "Produktlansering"

export enum ActivityStatus {
  NOTSTARTED = 0,
  ONGOING = 1,
  FINISHED = 2
}

//NOTSTARTED = "unchecked",
//  ONGOING = "crossed",
//  FINISHED = "checked"

export enum Rights {
  READ = 0,
  WRITE = 1,
  READWRITE = 2
}

//READ = "R",
//  WRITE = "W",
//  READWRITE = "RW"

export enum Role {
  OWNER = 0,
  PARTICIPANT = 1
}

//OWNER = "Projektägare",
//  PARTICIPANT = "Projektdeltagare",

export enum UserProjectStatus {
  PENDING = 0,
  ACCEPTED = 1,
  REJECTED = 2
}


