chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === "getJobDescription") {
      chrome.tabs.sendMessage(sender.tab.id, { action: "getJobDescription" }, sendResponse);
      return true;
    } else if (request.action === "jobDescription") {
      chrome.storage.session.set({ jobDescription: request.jobDescription });
    } else if (request.action === "sendData") {
      const fileData = request.fileData;
      const fileName = request.fileName;
      const jobDescription = request.jobDescription;
      sendDataToBackend(fileData, fileName, jobDescription, sendResponse);
      return true;
    }
  });
  
  function sendDataToBackend(fileData, fileName, jobDescription, sendResponse) {
    const formData = new FormData();
    const blob = new Blob([fileData]);
    formData.append("file", blob, fileName); // Key must be "file"
    formData.append("jobDescription", jobDescription); // Key must be "jobDescription"
    const apiUrl = "http://localhost:5250/api/resume/upload";
  
    fetch(apiUrl, {
      method: "POST",
      body: formData,
    })
      .then((response) => {
        if (!response.ok) {
          return response.text().then((text) => {
            throw new Error(`HTTP error! Status: ${response.status}, Response: ${text}`);
          });
        }
        return response.json();
      })
      .then((data) => {
        sendResponse({ success: true, results: data });
      })
      .catch((error) => {
        sendResponse({ success: false, error: error.message });
      });
  }