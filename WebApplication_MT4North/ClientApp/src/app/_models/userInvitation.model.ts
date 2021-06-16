export class UserInvitation {
  id: number;
  email: string;
  role: string;
  permissions: string;

  constructor(id: number, email: string, role: string, permissions: string) {
    this.id = id;
    this.email = email;
    this.role = role;
    this.permissions = permissions;
  }
}
