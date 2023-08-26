# 23-08-26

1. Entry 1:15 p.m - anjay
   - Added Calendar Management folder. This folder contains classes that handle Calendar data process
   - Problem :
      - Return dari event di calendar, timezone-nya UTC. Padahal setting di Calendar-nya udah GMT+7, dan listRequestnya udah diset Asia/Jakarta.
      - Kalo entry start DateTime dan end DateTime dari senin-rabu jam 12pm-3pm, event senin jam 10am-12pm malah masuk, dan event rabu jam 2pm-4pm ga masuk.