import Stack from "@mui/material/Stack"
import { memo, ReactNode } from "react"
import { useWizard } from "./Wizard"

const StepContent = ({
	children,
	stepId,
}: {
	children: ReactNode
	stepId?: number | string
}) => {
	const { currentStep } = useWizard()

	return (
		<Stack
			className="flex-1"
			sx={{
				gridArea: "1 / 1",
				visibility: stepId === currentStep ? "visible" : "hidden",
				pointerEvents: stepId === currentStep ? "auto" : "none",
			}}
		>
			{children}
		</Stack>
	)
}
export default memo(StepContent)
