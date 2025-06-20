const notifications = document.querySelector(".notifications");

const toastDetails = {
  timer: 3000,
  success: {
    icon: "fa-circle-check",
  },
  error: {
    icon: "fa-circle-xmark",
  },
  warning: {
    icon: "fa-triangle-exclamation",
  },
  info: {
    icon: "fa-circle-info",
  },
};

const removeToast = (toast) => {
  toast.classList.add("hideToast");
  if (toast.timeoutId) {
    clearTimeout(toast.timeoutId);
  }
  setTimeout(() => {
    toast.remove();
  }, 300);
};

const _showToast = (type, message) => {
  const { icon } = toastDetails[type] || toastDetails.info;
  const toast = document.createElement("li");
  toast.className = `toastify ${type}`;
  toast.innerHTML = `<div class="column">
                         <i class="fa-solid ${icon}"></i>
                         <span>${message}</span>
                      </div>
                      <i class="fa-solid fa-xmark" onclick="removeToast(this.parentElement)"></i>`;
  notifications.innerHTML = "";
  notifications.appendChild(toast);
  toast.timeoutId = setTimeout(() => removeToast(toast), toastDetails.timer);
};

const successToast = (message) => _showToast("success", message);
const errorToast = (message) => _showToast("error", message);
const warningToast = (message) => _showToast("warning", message);
const infoToast = (message) => _showToast("info", message);
