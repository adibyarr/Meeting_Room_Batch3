# 23-08-26

1. Entry 1:15 pm - anjay
   - Added Calendar Management folder. This folder contains classes that handle Calendar data process
   - Problem :
      - Return dari event di calendar, timezone-nya UTC. Padahal setting di Calendar-nya udah GMT+7, dan listRequestnya udah diset Asia/Jakarta.
        - [x] Solved
          - Default timezone dari akun utamanya kudu diset ke GMT+7 juga
      - Kalo entry start DateTime dan end DateTime dari senin-rabu jam 12pm-3pm, event senin jam 10am-12pm malah masuk, dan event rabu jam 2pm-4pm ga masuk.
        - [x] Solved
          - Masalah di timezone. (poin atas)

# 23-08-27

1. Entry 9.21 pm - anjay
   - Added GoogleOAuth.cs file to create oauth credential
   - Added Credentials folder to store credentials file
   - Modified some variable type in CalendarManager.cs and BookingController.cs, adjusting change from service account to oauth