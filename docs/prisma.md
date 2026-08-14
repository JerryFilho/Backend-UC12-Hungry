model User {
  id          Int           @id @default(autoincrement())
  name        String
  type        String  
  email       String        @unique
  password    String    
  birthDate   DateTime      @map("birth_date")
  cpf         String        @unique
 
  schedulles  Schedulle[]
  assessments Assessment[]
  companies   Company[]
  payments    Payment[]
  photos      Photo[]
 
  createdAt   DateTime      @default(now())
  updatedAt   DateTime      @updatedAt
 
  @@map("users")
}
 
model Schedulle {
  id          Int       @id @default(autoincrement())
  datetimez   DateTime
  type        String
  people      Int
  observation String?
 
  userId      Int       @map("user_id") 
  user        User      @relation(fields: [userId], references: [id])
 
  companyId   Int       @map("company_id")
  company     Company   @relation(fields: [companyId], references: [id])
 
  assessments Assessment[]
 
  createdAt   DateTime  @default(now())
  updatedAt   DateTime  @updatedAt
 
  @@map("schedulles")
}
 
model Company{
  id          Int         @id @default(autoincrement())
  name        String
  category    String
  cnpj        String      @unique
  places      String
  phone       String
  fundation   DateTime
  description String
 
  schedulles  Schedulle[]
  payments    Payment[]
 
  userId      Int?        @map("user_id")
  user        User?       @relation(fields: [userId], references: [id])
 
  photos      Photo[]
 
  createdAt   DateTime    @default(now())
  updatedAt   DateTime    @updatedAt
 
  @@map("companies")
}
 
model Photo{
  id            Int         @id @default(autoincrement())
  url           String      @unique
  companyId     Int         @map("company_id")
  company       Company     @relation(fields: [companyId], references: [id])
 
  userId        Int?        @map("user_id")
  user          User?       @relation(fields: [userId], references: [id])
 
  createdAt     DateTime    @default(now())
  updatedAt     DateTime    @updatedAt
 
  @@map("photos")
}
 
model Payment{
  id          Int         @id @default(autoincrement())
  value       Float       @default(0.00)
  due_date    DateTime    @map("due_date")
  to_date     DateTime    @map("to_date")
 
  companyId   Int
  company     Company     @relation(fields: [companyId], references: [id])
 
  userId      Int?        @map("user_id")
  user        User?       @relation(fields: [userId], references: [id])
 
  createdAt   DateTime    @default(now())
  updatedAt   DateTime    @updatedAt
 
  @@map("payment")
}
 
model Assessment {
  id              Int         @id @default(autoincrement())
  userId          Int
  user            User        @relation(fields: [userId], references: [id])
 
  schedullesId    Int
  schedulles      Schedulle   @relation(fields: [schedullesId], references: [id])
 
  note            Int  
  comment         String?
 
  createdAt       DateTime    @default(now())
  updatedAt       DateTime    @updatedAt
 
  @@map("assessment")
}

