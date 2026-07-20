import Stack from "@mui/material/Stack"
import { ReactNode } from "react"
import { useWizard } from "./Wizard"

export const StepContent = ({
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
