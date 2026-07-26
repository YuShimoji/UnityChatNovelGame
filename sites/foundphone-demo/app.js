(() => {
  "use strict";

  const REQUIRED_EVENT_NAMES = [
    "demo_started",
    "choice_selected",
    "demo_completed",
    "outbound_store_intent"
  ];
  const PACKAGE_SCHEMA = "foundphone.sites-preview-package";
  const PACKAGE_VERSION = 1;
  const POINTER_DRAG_THRESHOLD_PX = 8;
  const CONTENT_PATHS = Object.freeze({
    fixture: "./content/demo.json",
    generated: "./content/generated-preview.json"
  });

  const elements = {
    prototypeLabel: document.querySelector("#prototype-label"),
    introScreen: document.querySelector("#intro-screen"),
    introEyebrow: document.querySelector("#intro-eyebrow"),
    introTitle: document.querySelector("#intro-title"),
    introSummary: document.querySelector("#intro-summary"),
    contentOrigin: document.querySelector("#content-origin"),
    startButton: document.querySelector("#start-button"),
    loadStatus: document.querySelector("#load-status"),
    chatScreen: document.querySelector("#chat-screen"),
    threadTitle: document.querySelector("#thread-title"),
    toolbarRestartButton: document.querySelector("#toolbar-restart-button"),
    progressLabel: document.querySelector("#progress-label"),
    routeLabel: document.querySelector("#route-label"),
    progressTrack: document.querySelector(".progress-track"),
    progressFill: document.querySelector("#progress-fill"),
    chatSurface: document.querySelector("#chat-surface"),
    messageViewport: document.querySelector("#message-viewport"),
    messageList: document.querySelector("#message-list"),
    choiceRegion: document.querySelector("#choice-region"),
    choicePrompt: document.querySelector("#choice-prompt"),
    choiceList: document.querySelector("#choice-list"),
    advanceButton: document.querySelector("#advance-button"),
    endingScreen: document.querySelector("#ending-screen"),
    endingTitle: document.querySelector("#ending-title"),
    endingBody: document.querySelector("#ending-body"),
    routeOutcome: document.querySelector("#route-outcome"),
    endingRestartButton: document.querySelector("#ending-restart-button"),
    futureReleaseButton: document.querySelector("#future-release-button"),
    futureReleaseNote: document.querySelector("#future-release-note")
  };

  const state = {
    content: null,
    contentMode: "fixture",
    phase: "loading",
    nodeId: null,
    step: 0,
    choiceId: null,
    lineInProgress: false,
    advanceLocked: false,
    pointerGesture: null,
    eventLog: []
  };

  function recordEvent(name, detail = {}) {
    if (!REQUIRED_EVENT_NAMES.includes(name)) {
      throw new Error(`Unknown local event: ${name}`);
    }

    const event = {
      name,
      detail,
      localTimestampMs: Math.round(performance.now())
    };

    state.eventLog.push(event);
    window.dispatchEvent(new CustomEvent("foundphone:event", { detail: event }));
    console.info("[FoundPhone local event]", name, detail);
  }

  function showScreen(screenName) {
    elements.introScreen.hidden = screenName !== "intro";
    elements.chatScreen.hidden = screenName !== "chat";
    elements.endingScreen.hidden = screenName !== "ending";
    document.body.dataset.demoState = screenName;
  }

  function assertPreviewPackage(previewPackage) {
    if (previewPackage?.schema !== PACKAGE_SCHEMA || previewPackage?.version !== PACKAGE_VERSION) {
      throw new Error(`Unsupported Sites Preview Package schema/version`);
    }

    if (!previewPackage.nodeName || !previewPackage.source?.assetPath) {
      throw new Error("Sites Preview Package provenance is incomplete");
    }

    if (!Array.isArray(previewPackage.displayLines) || previewPackage.displayLines.length === 0) {
      throw new Error("Sites Preview Package has no display lines");
    }

    if (previewPackage.displayLines.some((line) => !line.text || !line.speakerId)) {
      throw new Error("Sites Preview Package contains an incomplete display line");
    }
  }

  function normalizePreviewPackage(previewPackage) {
    assertPreviewPackage(previewPackage);

    const participants = {
      system: { label: "SYSTEM", role: "system" },
      narrator: { label: "NARRATION", role: "system" },
      player: { label: "You", role: "player" }
    };
    const nodes = {};

    previewPackage.displayLines.forEach((line, index) => {
      const speakerId = line.speakerId;
      if (!participants[speakerId]) {
        participants[speakerId] = {
          label: line.speakerLabel || speakerId,
          role: speakerId === "player" ? "player" : "relay"
        };
      }

      const nodeId = `exported_line_${String(index + 1).padStart(3, "0")}`;
      const nextId = index + 1 < previewPackage.displayLines.length
        ? `exported_line_${String(index + 2).padStart(3, "0")}`
        : "exported_ending";
      nodes[nodeId] = {
        type: "message",
        speaker: speakerId,
        text: line.text,
        next: nextId,
        sourceLine: line.sourceLine,
        presentationKind: line.kind
      };
    });

    const diagnostics = Array.isArray(previewPackage.diagnostics)
      ? previewPackage.diagnostics
      : [];
    nodes.exported_ending = {
      type: "ending",
      heading: "Package preview complete",
      body: `${previewPackage.displayLines.length}本の表示行を順序どおり確認しました。`,
      defaultOutcome:
        `Package ${previewPackage.packageIdentitySha256.slice(0, 12)} / ` +
        `unsupported diagnostics ${diagnostics.length}`,
      outcomes: {}
    };

    return {
      meta: {
        id: previewPackage.packageIdentitySha256,
        version: `${previewPackage.version}`,
        contentLabel: previewPackage.contentLabel,
        canonStatus: previewPackage.canonStatus,
        locale: "ja-JP",
        contentMode: "generated",
        sourceNode: previewPackage.nodeName,
        sourceAssetPath: previewPackage.source.assetPath,
        sourceTitleLine: previewPackage.source.titleLine
      },
      intro: {
        eyebrow: "UNITY / YARN PACKAGE PREVIEW",
        heading: previewPackage.nodeName,
        summary:
          `${previewPackage.source.assetPath}:${previewPackage.source.titleLine}から出力した、` +
          "対応subset限定のローカルpreviewです。",
        startLabel: "Package previewを開始"
      },
      participants,
      flow: {
        start: "exported_line_001",
        totalSteps: previewPackage.displayLines.length + 1,
        nodes
      }
    };
  }

  function setLoadedContent(content, contentMode) {
    const normalizedContent = contentMode === "generated"
      ? normalizePreviewPackage(content)
      : content;

    state.content = normalizedContent;
    state.contentMode = contentMode;
    elements.prototypeLabel.textContent = normalizedContent.meta.contentLabel;
    elements.introEyebrow.textContent = normalizedContent.intro.eyebrow;
    elements.introTitle.textContent = normalizedContent.intro.heading;
    elements.introSummary.textContent = normalizedContent.intro.summary;
    elements.startButton.textContent = normalizedContent.intro.startLabel;
    elements.threadTitle.textContent = contentMode === "generated"
      ? normalizedContent.meta.sourceNode
      : normalizedContent.participants.relay?.label ?? "Unknown relay";
    elements.contentOrigin.textContent = contentMode === "generated"
      ? `Content: generated Package v1 / ${normalizedContent.meta.sourceAssetPath}:${normalizedContent.meta.sourceTitleLine}`
      : "Content: manually-authored non-canon fixture / content/demo.json";
    elements.startButton.disabled = false;
    elements.loadStatus.textContent = contentMode === "generated"
      ? "Unity/Yarnから生成したローカルpackageを読み込みました。"
      : "手動fixtureの準備ができました。";
    state.phase = "intro";
    showScreen("intro");
  }

  function resetConversation() {
    state.nodeId = state.content.flow.start;
    state.step = 0;
    state.choiceId = null;
    state.lineInProgress = false;
    state.advanceLocked = false;
    state.pointerGesture = null;
    elements.messageList.replaceChildren();
    elements.choiceList.replaceChildren();
    elements.choiceRegion.hidden = true;
    elements.advanceButton.hidden = true;
    elements.futureReleaseNote.hidden = true;
    elements.futureReleaseButton.setAttribute("aria-expanded", "false");
    updateProgress();
  }

  function startDemo() {
    if (!state.content) {
      return;
    }

    resetConversation();
    state.phase = "chat";
    showScreen("chat");
    recordEvent("demo_started", {
      contentVersion: state.content.meta.version,
      contentMode: state.contentMode
    });
    revealCurrentNode();
    elements.threadTitle.focus();
  }

  function updateProgress() {
    const total = state.content?.flow.totalSteps ?? 0;
    const boundedStep = Math.min(state.step, total);
    elements.progressLabel.textContent = `進行 ${boundedStep} / ${total}`;
    elements.progressTrack.setAttribute("aria-valuemax", String(total));
    elements.progressTrack.setAttribute("aria-valuenow", String(boundedStep));
    elements.progressFill.style.width = total === 0 ? "0%" : `${(boundedStep / total) * 100}%`;
    elements.routeLabel.textContent = state.contentMode === "generated"
      ? "Package v1"
      : state.choiceId
        ? `分岐: ${state.choiceId}`
        : "分岐: 未選択";
  }

  function participantFor(speakerId) {
    return state.content.participants[speakerId] ?? {
      label: speakerId,
      role: "system"
    };
  }

  function appendMessage(speakerId, text) {
    const participant = participantFor(speakerId);
    const item = document.createElement("li");
    item.className = `message message--${participant.role}`;
    item.setAttribute("aria-label", `${participant.label}からのメッセージ`);

    const speaker = document.createElement("p");
    speaker.className = "message-speaker";
    speaker.textContent = participant.label;

    const bubble = document.createElement("p");
    bubble.className = "message-bubble";
    bubble.textContent = text;

    item.append(speaker, bubble);
    elements.messageList.append(item);
    elements.messageViewport.scrollTop = elements.messageViewport.scrollHeight;
  }

  function configureAdvanceButton(nextNodeId) {
    const nextNode = state.content.flow.nodes[nextNodeId];
    elements.advanceButton.textContent = nextNode?.type === "choice"
      ? "返信を選ぶ"
      : nextNode?.type === "ending"
        ? "結果を見る"
        : "続ける";
    elements.advanceButton.hidden = false;
    elements.chatSurface.dataset.advanceReady = "true";
  }

  function revealMessage(node) {
    appendMessage(node.speaker, node.text);
    state.step += 1;
    state.nodeId = node.next;
    state.lineInProgress = false;
    updateProgress();
    configureAdvanceButton(node.next);
  }

  function revealChoice(node) {
    state.phase = "choice";
    elements.chatSurface.dataset.advanceReady = "false";
    elements.advanceButton.hidden = true;
    elements.choicePrompt.textContent = node.prompt;
    elements.choiceList.replaceChildren();

    for (const option of node.options) {
      const button = document.createElement("button");
      button.className = "choice-button";
      button.type = "button";
      button.textContent = option.label;
      button.dataset.choiceId = option.id;
      button.addEventListener("pointerdown", (event) => event.stopPropagation());
      button.addEventListener("pointerup", (event) => event.stopPropagation());
      button.addEventListener("click", (event) => {
        event.stopPropagation();
        selectChoice(option);
      });
      bindKeyboardActivation(button, () => selectChoice(option));
      elements.choiceList.append(button);
    }

    elements.choiceRegion.hidden = false;
    elements.choiceList.querySelector("button")?.focus();
  }

  function selectChoice(option) {
    state.choiceId = option.id;
    state.step += 1;
    state.phase = "chat";
    appendMessage("player", option.reply);
    recordEvent("choice_selected", {
      choiceId: option.id,
      nodeId: state.nodeId
    });
    state.nodeId = option.next;
    elements.choiceRegion.hidden = true;
    updateProgress();
    configureAdvanceButton(option.next);
  }

  function revealEnding(node) {
    state.step += 1;
    state.phase = "ending";
    elements.chatSurface.dataset.advanceReady = "false";
    updateProgress();
    elements.endingTitle.textContent = node.heading;
    elements.endingBody.textContent = node.body;
    elements.routeOutcome.textContent = node.outcomes[state.choiceId] ?? node.defaultOutcome;
    showScreen("ending");
    recordEvent("demo_completed", {
      choiceId: state.choiceId,
      steps: state.step,
      contentMode: state.contentMode
    });
    elements.endingTitle.focus();
  }

  function revealCurrentNode() {
    const node = state.content.flow.nodes[state.nodeId];
    if (!node) {
      throw new Error(`Demo node was not found: ${state.nodeId}`);
    }

    if (node.type === "message") {
      revealMessage(node);
      return;
    }

    if (node.type === "choice") {
      revealChoice(node);
      return;
    }

    if (node.type === "ending") {
      revealEnding(node);
      return;
    }

    throw new Error(`Unsupported demo node type: ${node.type}`);
  }

  function completeInProgressLine() {
    state.lineInProgress = false;
    elements.chatSurface.dataset.lineInProgress = "false";
  }

  function requestAdvance() {
    if (state.advanceLocked || state.phase !== "chat") {
      return false;
    }

    state.advanceLocked = true;
    try {
      if (state.lineInProgress) {
        completeInProgressLine();
      } else {
        revealCurrentNode();
      }
      return true;
    } finally {
      queueMicrotask(() => {
        state.advanceLocked = false;
      });
    }
  }

  function bindKeyboardActivation(element, action) {
    element.addEventListener("keydown", (event) => {
      if (event.key !== "Enter" && event.key !== " ") {
        return;
      }

      event.preventDefault();
      event.stopPropagation();
      action();
    });
  }

  function isProtectedAdvanceTarget(target) {
    return target instanceof Element && Boolean(target.closest(
      "button, a, input, select, textarea, [contenteditable='true'], [role='button']"
    ));
  }

  function hasSelectedText() {
    const selection = window.getSelection();
    return Boolean(selection && !selection.isCollapsed && selection.toString().trim());
  }

  function beginSurfacePointer(event) {
    if (event.button !== 0 || state.phase !== "chat" || isProtectedAdvanceTarget(event.target)) {
      state.pointerGesture = null;
      return;
    }

    state.pointerGesture = {
      pointerId: event.pointerId,
      startX: event.clientX,
      startY: event.clientY,
      moved: false
    };
  }

  function trackSurfacePointer(event) {
    const gesture = state.pointerGesture;
    if (!gesture || gesture.pointerId !== event.pointerId) {
      return;
    }

    const distance = Math.hypot(
      event.clientX - gesture.startX,
      event.clientY - gesture.startY
    );
    if (distance > POINTER_DRAG_THRESHOLD_PX) {
      gesture.moved = true;
    }
  }

  function endSurfacePointer(event) {
    const gesture = state.pointerGesture;
    state.pointerGesture = null;
    if (!gesture ||
        gesture.pointerId !== event.pointerId ||
        gesture.moved ||
        isProtectedAdvanceTarget(event.target) ||
        hasSelectedText()) {
      return;
    }

    requestAdvance();
  }

  function showFutureReleaseNote() {
    const isExpanded = elements.futureReleaseButton.getAttribute("aria-expanded") === "true";
    const nextExpanded = !isExpanded;
    elements.futureReleaseButton.setAttribute("aria-expanded", String(nextExpanded));
    elements.futureReleaseNote.hidden = !nextExpanded;

    if (nextExpanded) {
      recordEvent("outbound_store_intent", {
        destination: null,
        mode: "local_note_only"
      });
    }
  }

  function exposeTestState() {
    window.FoundPhoneDemo = Object.freeze({
      getState: () => ({
        phase: state.phase,
        contentMode: state.contentMode,
        nodeId: state.nodeId,
        step: state.step,
        choiceId: state.choiceId,
        messageCount: elements.messageList.children.length,
        lineInProgress: state.lineInProgress,
        eventNames: state.eventLog.map((event) => event.name)
      }),
      requiredEventNames: [...REQUIRED_EVENT_NAMES],
      pointerDragThresholdPx: POINTER_DRAG_THRESHOLD_PX,
      start: startDemo,
      advance: requestAdvance,
      restart: startDemo
    });
  }

  async function initialize() {
    try {
      const requestedMode = new URLSearchParams(window.location.search).get("content") ?? "fixture";
      if (!Object.hasOwn(CONTENT_PATHS, requestedMode)) {
        throw new Error(`Unknown local content mode: ${requestedMode}`);
      }

      const response = await fetch(CONTENT_PATHS[requestedMode], { cache: "no-store" });
      if (!response.ok) {
        throw new Error(`Content request failed with HTTP ${response.status}`);
      }

      const content = await response.json();
      setLoadedContent(content, requestedMode);
    } catch (error) {
      state.phase = "error";
      elements.loadStatus.classList.add("is-error");
      elements.loadStatus.textContent = `デモを読み込めませんでした: ${error.message}`;
      console.error(error);
    }
  }

  elements.startButton.addEventListener("click", startDemo);
  elements.toolbarRestartButton.addEventListener("click", startDemo);
  elements.advanceButton.addEventListener("click", (event) => {
    event.stopPropagation();
    requestAdvance();
  });
  bindKeyboardActivation(elements.advanceButton, requestAdvance);
  elements.endingRestartButton.addEventListener("click", startDemo);
  elements.futureReleaseButton.addEventListener("click", showFutureReleaseNote);
  elements.chatSurface.addEventListener("pointerdown", beginSurfacePointer);
  elements.chatSurface.addEventListener("pointermove", trackSurfacePointer);
  elements.chatSurface.addEventListener("pointerup", endSurfacePointer);
  elements.chatSurface.addEventListener("pointercancel", () => {
    state.pointerGesture = null;
  });

  exposeTestState();
  initialize();
})();
